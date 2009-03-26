#include <ntifs.h>

#define KBC_DEVICE_TYPE (0x9998)
#define KBC_DEVICE_NAME (L"\\Device\\KBugCheck")
#define KBC_DEVICE_DOS_NAME (L"\\DosDevices\\KBugCheck")
#define KBC_CTL_CODE(x) CTL_CODE(KBC_DEVICE_TYPE, 0x801 + x, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define KBC_BUGCHECKEX KBC_CTL_CODE(0)

void DriverUnload(PDRIVER_OBJECT DriverObject);
NTSTATUS DriverEntry(PDRIVER_OBJECT DriverObject, PUNICODE_STRING RegistryPath);
NTSTATUS DeviceCreate(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS DeviceClose(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS DeviceIoControl(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS DeviceRead(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS DeviceUnsupported(PDEVICE_OBJECT DeviceObject, PIRP Irp);

#pragma alloc_text(PAGE, DeviceCreate)
#pragma alloc_text(PAGE, DeviceClose)
#pragma alloc_text(PAGE, DeviceIoControl)
#pragma alloc_text(PAGE, DeviceRead)
#pragma alloc_text(PAGE, DeviceUnsupported)

void DriverUnload(PDRIVER_OBJECT DriverObject)
{
    UNICODE_STRING dosDeviceName;
    
    RtlInitUnicodeString(&dosDeviceName, KBC_DEVICE_DOS_NAME);
    IoDeleteSymbolicLink(&dosDeviceName);
    IoDeleteDevice(DriverObject->DeviceObject);
}

NTSTATUS DriverEntry(PDRIVER_OBJECT DriverObject, PUNICODE_STRING RegistryPath)
{
    NTSTATUS status = STATUS_SUCCESS;
    int i;
    PDEVICE_OBJECT deviceObject = NULL;
    UNICODE_STRING deviceName, dosDeviceName;
    
    RtlInitUnicodeString(&deviceName, KBC_DEVICE_NAME);
    RtlInitUnicodeString(&dosDeviceName, KBC_DEVICE_DOS_NAME);
    
    status = IoCreateDevice(DriverObject, 0, &deviceName, 
        FILE_DEVICE_UNKNOWN, FILE_DEVICE_SECURE_OPEN, FALSE, &deviceObject);
    
    for (i = 0; i < IRP_MJ_MAXIMUM_FUNCTION; i++)
        DriverObject->MajorFunction[i] = NULL;
    
    DriverObject->MajorFunction[IRP_MJ_CLOSE] = DeviceClose;
    DriverObject->MajorFunction[IRP_MJ_CREATE] = DeviceCreate;
    DriverObject->MajorFunction[IRP_MJ_READ] = DeviceRead;
    DriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = DeviceIoControl;
    DriverObject->DriverUnload = DriverUnload;
    
    deviceObject->Flags |= DO_BUFFERED_IO;
    deviceObject->Flags &= ~DO_DEVICE_INITIALIZING;
    
    IoCreateSymbolicLink(&dosDeviceName, &deviceName);
    
    return STATUS_SUCCESS;
}

NTSTATUS DeviceCreate(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    
    if (!SeSinglePrivilegeCheck(RtlConvertLongToLuid(SE_DEBUG_PRIVILEGE), UserMode))
        status = STATUS_ACCESS_DENIED;
    
    return status;
}

NTSTATUS DeviceClose(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    
    return status;
}

NTSTATUS DeviceIoControl(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    PIO_STACK_LOCATION ioStackIrp = NULL;
    PCHAR dataBuffer;
    int controlCode;
    unsigned int inLength, outLength;
    int retLength = 0;
    
    Irp->IoStatus.Status = STATUS_SUCCESS;
    Irp->IoStatus.Information = 0;
    
    ioStackIrp = IoGetCurrentIrpStackLocation(Irp);
    
    if (ioStackIrp == NULL)
    {
        status = STATUS_INTERNAL_ERROR;
        goto IoControlEnd;
    }
    
    dataBuffer = (PCHAR)Irp->AssociatedIrp.SystemBuffer;
    
    if (dataBuffer == NULL)
    {
        status = STATUS_INTERNAL_ERROR;
        goto IoControlEnd;
    }
    
    inLength = ioStackIrp->Parameters.DeviceIoControl.InputBufferLength;
    outLength = ioStackIrp->Parameters.DeviceIoControl.OutputBufferLength;
    controlCode = ioStackIrp->Parameters.DeviceIoControl.IoControlCode;
    
    switch (controlCode)
    {
        case KBC_BUGCHECKEX:
        {
            ULONG code;
            ULONG_PTR param1, param2, param3, param4;
            
            if (inLength < (sizeof(ULONG) + sizeof(ULONG_PTR) * 4))
            {
                status = STATUS_BUFFER_TOO_SMALL;
                goto IoControlEnd;
            }
            
            code = *(ULONG *)(dataBuffer);
            param1 = *(ULONG_PTR *)(dataBuffer + sizeof(ULONG));
            param2 = *(ULONG_PTR *)(dataBuffer + sizeof(ULONG) + sizeof(ULONG_PTR));
            param3 = *(ULONG_PTR *)(dataBuffer + sizeof(ULONG) + sizeof(ULONG_PTR) * 2);
            param4 = *(ULONG_PTR *)(dataBuffer + sizeof(ULONG) + sizeof(ULONG_PTR) * 3);
            
            KeBugCheckEx(code, param1, param2, param3, param4);
        }
        break;
        
        default:
        {
            status = STATUS_INVALID_DEVICE_REQUEST;
        }
        break;
    }
    
IoControlEnd:
    Irp->IoStatus.Information = retLength;
    Irp->IoStatus.Status = status;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    
    return status;
}

NTSTATUS DeviceRead(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status = STATUS_SUCCESS;
    PIO_STACK_LOCATION ioStackIrp = NULL;
    int retLength = 0;
    
    ioStackIrp = IoGetCurrentIrpStackLocation(Irp);
    
    if (ioStackIrp != NULL)
    {
        PCHAR readDataBuffer = (PCHAR)Irp->AssociatedIrp.SystemBuffer;
        int readLength = ioStackIrp->Parameters.Read.Length;
        
        if (readDataBuffer != NULL)
        {
            if (readLength == 4)
            {
                *(int *)readDataBuffer = KBC_CTL_CODE(0);
                retLength = 4;
            }
            else
            {
                status = STATUS_INFO_LENGTH_MISMATCH;
            }
        }
    }
    
    Irp->IoStatus.Information = retLength;
    Irp->IoStatus.Status = status;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    
    return status;
}

NTSTATUS DeviceUnsupported(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    return STATUS_NOT_IMPLEMENTED;
}
