/*
 * Process Hacker - 
 *   main Process Hacker window
 * 
 * Copyright (C) 2008-2009 Dean
 * Copyright (C) 2008-2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Aga.Controls.Tree;
using ProcessHacker.Common;
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Debugging;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;
using ProcessHacker.UI.Actions;

namespace ProcessHacker
{
    public partial class HackerWindow : Form
    {
        public delegate void LogUpdatedEventHandler(KeyValuePair<DateTime, string>? value);

        private delegate void AddMenuItemDelegate(string text, EventHandler onClick);

        // This entire file is a big monolithic mess.

        #region "MenuItems"

        private MenuItem terminateMenuItem;
        private MenuItem suspendMenuItem;
        private MenuItem resumeMenuItem;
        private MenuItem menuItem5;
        private MenuItem priorityMenuItem;
        private MenuItem menuItem7;
        private MenuItem realTimeMenuItem;
        private MenuItem highMenuItem;
        private MenuItem aboveNormalMenuItem;
        private MenuItem normalMenuItem;
        private MenuItem belowNormalMenuItem;
        private MenuItem idleMenuItem;
        private MenuItem hackerMenuItem;
        private MenuItem aboutMenuItem;
        private MenuItem optionsMenuItem;
        private MenuItem helpMenuItem;
        private MenuItem exitMenuItem;
        private MenuItem windowMenuItem;
        private MenuItem inspectPEFileMenuItem;
        private MenuItem propertiesProcessMenuItem;
        private MenuItem searchProcessMenuItem;
        private MenuItem propertiesServiceMenuItem;
        private MenuItem startServiceMenuItem;
        private MenuItem pauseServiceMenuItem;
        private MenuItem stopServiceMenuItem;
        private MenuItem deleteServiceMenuItem;
        private MenuItem continueServiceMenuItem;
        private MenuItem goToProcessServiceMenuItem;
        private MenuItem menuItem8;
        private MenuItem copyServiceMenuItem;
        private MenuItem selectAllServiceMenuItem;
        private MenuItem toolsMenuItem;
        private MenuItem showHideMenuItem;
        private MenuItem exitTrayMenuItem;
        private MenuItem notificationsMenuItem;
        private MenuItem NPMenuItem;
        private MenuItem TPMenuItem;
        private MenuItem NSMenuItem;
        private MenuItem startedSMenuItem;
        private MenuItem stoppedSMenuItem;
        private MenuItem DSMenuItem;
        private MenuItem findHandlesMenuItem;
        private MenuItem affinityProcessMenuItem;
        private MenuItem runAsServiceMenuItem;
        private MenuItem runAsProcessMenuItem;
        private MenuItem launchAsUserProcessMenuItem;
        private MenuItem launchAsThisUserProcessMenuItem;
        private MenuItem sysInfoMenuItem;
        private MenuItem copyProcessMenuItem;
        private MenuItem selectAllProcessMenuItem;
        private MenuItem terminatorProcessMenuItem;
        private MenuItem menuItem2;
        private MenuItem logMenuItem;
        private MenuItem reloadStructsMenuItem;
        private MenuItem sysInformationIconMenuItem;
        private MenuItem hiddenProcessesMenuItem;
        private MenuItem viewMenuItem;
        private MenuItem updateNowMenuItem;
        private MenuItem updateProcessesMenuItem;
        private MenuItem updateServicesMenuItem;
        private MenuItem processesMenuItem;
        private MenuItem restartProcessMenuItem;
        private MenuItem setTokenProcessMenuItem;
        private MenuItem helpMenu;
        private MenuItem menuItem3;
        private MenuItem verifyFileSignatureMenuItem;
        private MenuItem enableAllNotificationsMenuItem;
        private MenuItem disableAllNotificationsMenuItem;
        private MenuItem menuItem4;
        private MenuItem shutdownTrayMenuItem;
        private MenuItem shutdownMenuItem;
        private MenuItem runAsAdministratorMenuItem;
        private MenuItem showDetailsForAllProcessesMenuItem;
        private MenuItem uacSeparatorMenuItem;
        private MenuItem runMenuItem;
        private MenuItem runAsMenuItem;
        private MenuItem freeMemoryMenuItem;
        private MenuItem menuItem1;
        private MenuItem reanalyzeProcessMenuItem;
        private MenuItem reduceWorkingSetProcessMenuItem;
        private MenuItem virtualizationProcessMenuItem;
        private MenuItem toolbarMenuItem;
        private MenuItem saveMenuItem;
        private MenuItem goToProcessNetworkMenuItem;
        private MenuItem copyNetworkMenuItem;
        private MenuItem menuItem6;
        private MenuItem selectAllNetworkMenuItem;
        private MenuItem injectDllProcessMenuItem;
        private MenuItem terminateProcessTreeMenuItem;
        private MenuItem trayIconsMenuItem;
        private MenuItem cpuHistoryMenuItem;
        private MenuItem cpuUsageMenuItem;
        private MenuItem ioHistoryMenuItem;
        private MenuItem commitHistoryMenuItem;
        private MenuItem physMemHistoryMenuItem;
        private MenuItem closeNetworkMenuItem;
        private MenuItem protectionProcessMenuItem;
        private MenuItem createDumpFileProcessMenuItem;
        private MenuItem miscellaneousProcessMenuItem;
        private MenuItem detachFromDebuggerProcessMenuItem;
        private MenuItem usersMenuItem;
        private MenuItem createServiceMenuItem;
        private MenuItem heapsProcessMenuItem;
        private MenuItem windowProcessMenuItem;
        private MenuItem bringToFrontProcessMenuItem;
        private MenuItem restoreProcessMenuItem;
        private MenuItem minimizeProcessMenuItem;
        private MenuItem maximizeProcessMenuItem;
        private MenuItem menuItem15;
        private MenuItem closeProcessMenuItem;
        private MenuItem checkForUpdatesMenuItem;
        private MenuItem toolsNetworkMenuItem;
        private MenuItem whoisNetworkMenuItem;
        private MenuItem tracertNetworkMenuItem;
        private MenuItem pingNetworkMenuItem;

        #endregion

        private void CreateHackerMainMenu()
        {
            MainMenu HackerMenu = new MainMenu();

            this.hackerMenuItem = new MenuItem();
            this.runMenuItem = new MenuItem();
            this.runAsAdministratorMenuItem = new MenuItem();
            this.runAsMenuItem = new MenuItem();
            this.runAsServiceMenuItem = new MenuItem();
            this.showDetailsForAllProcessesMenuItem = new MenuItem();
            this.uacSeparatorMenuItem = new MenuItem();
            this.saveMenuItem = new MenuItem();
            this.findHandlesMenuItem = new MenuItem();
            this.inspectPEFileMenuItem = new MenuItem();
            this.reloadStructsMenuItem = new MenuItem();
            this.optionsMenuItem = new MenuItem();
            this.menuItem2 = new MenuItem();
            this.shutdownMenuItem = new MenuItem();
            this.exitMenuItem = new MenuItem();
            this.viewMenuItem = new MenuItem();
            this.toolbarMenuItem = new MenuItem();
            this.sysInfoMenuItem = new MenuItem();
            this.trayIconsMenuItem = new MenuItem();
            this.cpuHistoryMenuItem = new MenuItem();
            this.cpuUsageMenuItem = new MenuItem();
            this.ioHistoryMenuItem = new MenuItem();
            this.commitHistoryMenuItem = new MenuItem();
            this.physMemHistoryMenuItem = new MenuItem();
            this.menuItem3 = new MenuItem();
            this.updateNowMenuItem = new MenuItem();
            this.updateProcessesMenuItem = new MenuItem();
            this.updateServicesMenuItem = new MenuItem();
            this.toolsMenuItem = new MenuItem();
            this.createServiceMenuItem = new MenuItem();
            this.hiddenProcessesMenuItem = new MenuItem();
            this.verifyFileSignatureMenuItem = new MenuItem();
            this.usersMenuItem = new MenuItem();
            this.windowMenuItem = new MenuItem();
            this.helpMenu = new MenuItem();
            this.freeMemoryMenuItem = new MenuItem();
            this.checkForUpdatesMenuItem = new MenuItem();
            this.menuItem1 = new MenuItem();
            this.logMenuItem = new MenuItem();
            this.helpMenuItem = new MenuItem();
            this.aboutMenuItem = new MenuItem();
            this.goToProcessServiceMenuItem = new MenuItem();
            this.startServiceMenuItem = new MenuItem();
            this.continueServiceMenuItem = new MenuItem();
            this.pauseServiceMenuItem = new MenuItem();
            this.stopServiceMenuItem = new MenuItem();
            this.deleteServiceMenuItem = new MenuItem();
            this.propertiesServiceMenuItem = new MenuItem();
            this.menuItem8 = new MenuItem();
            this.copyServiceMenuItem = new MenuItem();
            this.selectAllServiceMenuItem = new MenuItem();
            this.showHideMenuItem = new MenuItem();
            this.sysInformationIconMenuItem = new MenuItem();
            this.notificationsMenuItem = new MenuItem();
            this.enableAllNotificationsMenuItem = new MenuItem();
            this.disableAllNotificationsMenuItem = new MenuItem();
            this.menuItem4 = new MenuItem();
            this.NPMenuItem = new MenuItem();
            this.TPMenuItem = new MenuItem();
            this.NSMenuItem = new MenuItem();
            this.startedSMenuItem = new MenuItem();
            this.stoppedSMenuItem = new MenuItem();
            this.DSMenuItem = new MenuItem();
            this.processesMenuItem = new MenuItem();
            this.shutdownTrayMenuItem = new MenuItem();
            this.exitTrayMenuItem = new MenuItem();
            this.goToProcessNetworkMenuItem = new MenuItem();
            this.copyNetworkMenuItem = new MenuItem();
            this.closeNetworkMenuItem = new MenuItem();
            this.toolsNetworkMenuItem = new MenuItem();
            this.whoisNetworkMenuItem = new MenuItem();
            this.tracertNetworkMenuItem = new MenuItem();
            this.pingNetworkMenuItem = new MenuItem();
            this.menuItem6 = new MenuItem();
            this.selectAllNetworkMenuItem = new MenuItem();
            this.terminateMenuItem = new MenuItem();
            this.terminateProcessTreeMenuItem = new MenuItem();
            this.suspendMenuItem = new MenuItem();
            this.resumeMenuItem = new MenuItem();
            this.restartProcessMenuItem = new MenuItem();
            this.reduceWorkingSetProcessMenuItem = new MenuItem();
            this.virtualizationProcessMenuItem = new MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.affinityProcessMenuItem = new MenuItem();
            this.createDumpFileProcessMenuItem = new MenuItem();
            this.terminatorProcessMenuItem = new MenuItem();
            this.miscellaneousProcessMenuItem = new MenuItem();
            this.detachFromDebuggerProcessMenuItem = new MenuItem();
            this.heapsProcessMenuItem = new MenuItem();
            this.injectDllProcessMenuItem = new MenuItem();
            this.protectionProcessMenuItem = new MenuItem();
            this.setTokenProcessMenuItem = new MenuItem();
            this.priorityMenuItem = new MenuItem();
            this.realTimeMenuItem = new MenuItem();
            this.highMenuItem = new MenuItem();
            this.aboveNormalMenuItem = new MenuItem();
            this.normalMenuItem = new MenuItem();
            this.belowNormalMenuItem = new MenuItem();
            this.idleMenuItem = new MenuItem();
            this.runAsProcessMenuItem = new MenuItem();
            this.launchAsUserProcessMenuItem = new MenuItem();
            this.launchAsThisUserProcessMenuItem = new MenuItem();
            this.windowProcessMenuItem = new MenuItem();
            this.bringToFrontProcessMenuItem = new MenuItem();
            this.restoreProcessMenuItem = new MenuItem();
            this.minimizeProcessMenuItem = new MenuItem();
            this.maximizeProcessMenuItem = new MenuItem();
            this.menuItem15 = new MenuItem();
            this.closeProcessMenuItem = new MenuItem();
            this.propertiesProcessMenuItem = new MenuItem();
            this.menuItem7 = new MenuItem();
            this.searchProcessMenuItem = new MenuItem();
            this.reanalyzeProcessMenuItem = new MenuItem();
            this.copyProcessMenuItem = new MenuItem();
            this.selectAllProcessMenuItem = new MenuItem();

            Menu = HackerMenu;

            HackerMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.hackerMenuItem,
            this.viewMenuItem,
            this.toolsMenuItem,
            this.usersMenuItem,
            this.windowMenuItem,
            this.helpMenu});
            // 
            // menuProcess
            // 
            this.menuProcess.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.terminateMenuItem,
            this.terminateProcessTreeMenuItem,
            this.suspendMenuItem,
            this.resumeMenuItem,
            this.restartProcessMenuItem,
            this.reduceWorkingSetProcessMenuItem,
            this.virtualizationProcessMenuItem,
            this.menuItem5,
            this.affinityProcessMenuItem,
            this.createDumpFileProcessMenuItem,
            this.terminatorProcessMenuItem,
            this.miscellaneousProcessMenuItem,
            this.priorityMenuItem,
            this.runAsProcessMenuItem,
            this.windowProcessMenuItem,
            this.propertiesProcessMenuItem,
            this.menuItem7,
            this.searchProcessMenuItem,
            this.reanalyzeProcessMenuItem,
            this.copyProcessMenuItem,
            this.selectAllProcessMenuItem});
            this.menuProcess.Popup += new System.EventHandler(this.menuProcess_Popup);
            // 
            // terminateMenuItem
            // 
            this.vistaMenu.SetImage(this.terminateMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.terminateMenuItem.Index = 0;
            this.terminateMenuItem.Shortcut = System.Windows.Forms.Shortcut.Del;
            this.terminateMenuItem.Text = "&Terminate";
            this.terminateMenuItem.Click += new System.EventHandler(this.terminateMenuItem_Click);
            // 
            // terminateProcessTreeMenuItem
            // 
            this.terminateProcessTreeMenuItem.Index = 1;
            this.terminateProcessTreeMenuItem.Text = "Terminate Process Tree";
            this.terminateProcessTreeMenuItem.Click += new System.EventHandler(this.terminateProcessTreeMenuItem_Click);
            // 
            // suspendMenuItem
            // 
            this.vistaMenu.SetImage(this.suspendMenuItem, global::ProcessHacker.Properties.Resources.control_pause_blue);
            this.suspendMenuItem.Index = 2;
            this.suspendMenuItem.Text = "&Suspend";
            this.suspendMenuItem.Click += new System.EventHandler(this.suspendMenuItem_Click);
            // 
            // resumeMenuItem
            // 
            this.vistaMenu.SetImage(this.resumeMenuItem, global::ProcessHacker.Properties.Resources.control_play_blue);
            this.resumeMenuItem.Index = 3;
            this.resumeMenuItem.Text = "&Resume";
            this.resumeMenuItem.Click += new System.EventHandler(this.resumeMenuItem_Click);
            // 
            // restartProcessMenuItem
            // 
            this.restartProcessMenuItem.Index = 4;
            this.restartProcessMenuItem.Text = "Restart";
            this.restartProcessMenuItem.Click += new System.EventHandler(this.restartProcessMenuItem_Click);
            // 
            // reduceWorkingSetProcessMenuItem
            // 
            this.reduceWorkingSetProcessMenuItem.Index = 5;
            this.reduceWorkingSetProcessMenuItem.Text = "Reduce Working Set";
            this.reduceWorkingSetProcessMenuItem.Click += new System.EventHandler(this.reduceWorkingSetProcessMenuItem_Click);
            // 
            // virtualizationProcessMenuItem
            // 
            this.virtualizationProcessMenuItem.Index = 6;
            this.virtualizationProcessMenuItem.Text = "Virtualization";
            this.virtualizationProcessMenuItem.Click += new System.EventHandler(this.virtualizationProcessMenuItem_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 7;
            this.menuItem5.Text = "-";
            // 
            // affinityProcessMenuItem
            // 
            this.affinityProcessMenuItem.Index = 8;
            this.affinityProcessMenuItem.Text = "Affinity...";
            this.affinityProcessMenuItem.Click += new System.EventHandler(this.affinityProcessMenuItem_Click);
            // 
            // createDumpFileProcessMenuItem
            // 
            this.createDumpFileProcessMenuItem.Index = 9;
            this.createDumpFileProcessMenuItem.Text = "Create Dump File...";
            this.createDumpFileProcessMenuItem.Click += new System.EventHandler(this.createDumpFileProcessMenuItem_Click);
            // 
            // terminatorProcessMenuItem
            // 
            this.terminatorProcessMenuItem.Index = 10;
            this.terminatorProcessMenuItem.Text = "Terminator";
            this.terminatorProcessMenuItem.Click += new System.EventHandler(this.terminatorProcessMenuItem_Click);
            // 
            // miscellaneousProcessMenuItem
            // 
            this.miscellaneousProcessMenuItem.Index = 11;
            this.miscellaneousProcessMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.detachFromDebuggerProcessMenuItem,
            this.heapsProcessMenuItem,
            this.injectDllProcessMenuItem,
            this.protectionProcessMenuItem,
            this.setTokenProcessMenuItem});
            this.miscellaneousProcessMenuItem.Text = "Miscellaneous";
            // 
            // detachFromDebuggerProcessMenuItem
            // 
            this.detachFromDebuggerProcessMenuItem.Index = 0;
            this.detachFromDebuggerProcessMenuItem.Text = "Detach from Debugger";
            this.detachFromDebuggerProcessMenuItem.Click += new System.EventHandler(this.detachFromDebuggerProcessMenuItem_Click);
            // 
            // heapsProcessMenuItem
            // 
            this.heapsProcessMenuItem.Index = 1;
            this.heapsProcessMenuItem.Text = "Heaps";
            this.heapsProcessMenuItem.Click += new System.EventHandler(this.heapsProcessMenuItem_Click);
            // 
            // injectDllProcessMenuItem
            // 
            this.injectDllProcessMenuItem.Index = 2;
            this.injectDllProcessMenuItem.Text = "Inject DLL...";
            this.injectDllProcessMenuItem.Click += new System.EventHandler(this.injectDllProcessMenuItem_Click);
            // 
            // protectionProcessMenuItem
            // 
            this.protectionProcessMenuItem.Index = 3;
            this.protectionProcessMenuItem.Text = "Protection";
            this.protectionProcessMenuItem.Click += new System.EventHandler(this.protectionProcessMenuItem_Click);
            // 
            // setTokenProcessMenuItem
            // 
            this.setTokenProcessMenuItem.Index = 4;
            this.setTokenProcessMenuItem.Text = "Set Token...";
            this.setTokenProcessMenuItem.Click += new System.EventHandler(this.setTokenProcessMenuItem_Click);
            // 
            // priorityMenuItem
            // 
            this.vistaMenu.SetImage(this.priorityMenuItem, global::ProcessHacker.Properties.Resources.control_equalizer_blue);
            this.priorityMenuItem.Index = 12;
            this.priorityMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.realTimeMenuItem,
            this.highMenuItem,
            this.aboveNormalMenuItem,
            this.normalMenuItem,
            this.belowNormalMenuItem,
            this.idleMenuItem});
            this.priorityMenuItem.Text = "&Priority";
            // 
            // realTimeMenuItem
            // 
            this.realTimeMenuItem.Index = 0;
            this.realTimeMenuItem.RadioCheck = true;
            this.realTimeMenuItem.Text = "Real Time";
            this.realTimeMenuItem.Click += new System.EventHandler(this.realTimeMenuItem_Click);
            // 
            // highMenuItem
            // 
            this.highMenuItem.Index = 1;
            this.highMenuItem.RadioCheck = true;
            this.highMenuItem.Text = "High";
            this.highMenuItem.Click += new System.EventHandler(this.highMenuItem_Click);
            // 
            // aboveNormalMenuItem
            // 
            this.aboveNormalMenuItem.Index = 2;
            this.aboveNormalMenuItem.RadioCheck = true;
            this.aboveNormalMenuItem.Text = "Above Normal";
            this.aboveNormalMenuItem.Click += new System.EventHandler(this.aboveNormalMenuItem_Click);
            // 
            // normalMenuItem
            // 
            this.normalMenuItem.Index = 3;
            this.normalMenuItem.RadioCheck = true;
            this.normalMenuItem.Text = "Normal";
            this.normalMenuItem.Click += new System.EventHandler(this.normalMenuItem_Click);
            // 
            // belowNormalMenuItem
            // 
            this.belowNormalMenuItem.Index = 4;
            this.belowNormalMenuItem.RadioCheck = true;
            this.belowNormalMenuItem.Text = "Below Normal";
            this.belowNormalMenuItem.Click += new System.EventHandler(this.belowNormalMenuItem_Click);
            // 
            // idleMenuItem
            // 
            this.idleMenuItem.Index = 5;
            this.idleMenuItem.RadioCheck = true;
            this.idleMenuItem.Text = "Idle";
            this.idleMenuItem.Click += new System.EventHandler(this.idleMenuItem_Click);
            // 
            // runAsProcessMenuItem
            // 
            this.runAsProcessMenuItem.Index = 13;
            this.runAsProcessMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.launchAsUserProcessMenuItem,
            this.launchAsThisUserProcessMenuItem});
            this.runAsProcessMenuItem.Text = "Run As";
            // 
            // launchAsUserProcessMenuItem
            // 
            this.launchAsUserProcessMenuItem.Index = 0;
            this.launchAsUserProcessMenuItem.Text = "Launch As User...";
            this.launchAsUserProcessMenuItem.Click += new System.EventHandler(this.launchAsUserProcessMenuItem_Click);
            // 
            // launchAsThisUserProcessMenuItem
            // 
            this.launchAsThisUserProcessMenuItem.Index = 1;
            this.launchAsThisUserProcessMenuItem.Text = "Launch As This User...";
            this.launchAsThisUserProcessMenuItem.Click += new System.EventHandler(this.launchAsThisUserProcessMenuItem_Click);
            // 
            // windowProcessMenuItem
            // 
            this.windowProcessMenuItem.Index = 14;
            this.windowProcessMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.bringToFrontProcessMenuItem,
            this.restoreProcessMenuItem,
            this.minimizeProcessMenuItem,
            this.maximizeProcessMenuItem,
            this.menuItem15,
            this.closeProcessMenuItem});
            this.windowProcessMenuItem.Text = "&Window";
            // 
            // bringToFrontProcessMenuItem
            // 
            this.bringToFrontProcessMenuItem.Index = 0;
            this.bringToFrontProcessMenuItem.Text = "&Bring to Front";
            this.bringToFrontProcessMenuItem.Click += new System.EventHandler(this.bringToFrontProcessMenuItem_Click);
            // 
            // restoreProcessMenuItem
            // 
            this.restoreProcessMenuItem.Index = 1;
            this.restoreProcessMenuItem.Text = "&Restore";
            this.restoreProcessMenuItem.Click += new System.EventHandler(this.restoreProcessMenuItem_Click);
            // 
            // minimizeProcessMenuItem
            // 
            this.minimizeProcessMenuItem.Index = 2;
            this.minimizeProcessMenuItem.Text = "&Minimize";
            this.minimizeProcessMenuItem.Click += new System.EventHandler(this.minimizeProcessMenuItem_Click);
            // 
            // maximizeProcessMenuItem
            // 
            this.maximizeProcessMenuItem.Index = 3;
            this.maximizeProcessMenuItem.Text = "Ma&ximize";
            this.maximizeProcessMenuItem.Click += new System.EventHandler(this.maximizeProcessMenuItem_Click);
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 4;
            this.menuItem15.Text = "-";
            // 
            // closeProcessMenuItem
            // 
            this.closeProcessMenuItem.Index = 5;
            this.closeProcessMenuItem.Text = "&Close";
            this.closeProcessMenuItem.Click += new System.EventHandler(this.closeProcessMenuItem_Click);
            // 
            // propertiesProcessMenuItem
            // 
            this.propertiesProcessMenuItem.DefaultItem = true;
            this.vistaMenu.SetImage(this.propertiesProcessMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.propertiesProcessMenuItem.Index = 15;
            this.propertiesProcessMenuItem.Text = "&Properties";
            this.propertiesProcessMenuItem.Click += new System.EventHandler(this.propertiesProcessMenuItem_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 16;
            this.menuItem7.Text = "-";
            // 
            // searchProcessMenuItem
            // 
            this.searchProcessMenuItem.Index = 17;
            this.searchProcessMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlM;
            this.searchProcessMenuItem.Text = "&Search Online";
            this.searchProcessMenuItem.Click += new System.EventHandler(this.searchProcessMenuItem_Click);
            // 
            // reanalyzeProcessMenuItem
            // 
            this.reanalyzeProcessMenuItem.Index = 18;
            this.reanalyzeProcessMenuItem.Text = "Re-analyze";
            this.reanalyzeProcessMenuItem.Click += new System.EventHandler(this.reanalyzeProcessMenuItem_Click);
            // 
            // copyProcessMenuItem
            // 
            this.vistaMenu.SetImage(this.copyProcessMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyProcessMenuItem.Index = 19;
            this.copyProcessMenuItem.Text = "&Copy";
            // 
            // selectAllProcessMenuItem
            // 
            this.selectAllProcessMenuItem.Index = 20;
            this.selectAllProcessMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.selectAllProcessMenuItem.Text = "Select &All";
            this.selectAllProcessMenuItem.Click += new System.EventHandler(this.selectAllProcessMenuItem_Click);
            // 
            // hackerMenuItem
            // 
            this.hackerMenuItem.Index = 0;
            this.hackerMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.runMenuItem,
            this.runAsAdministratorMenuItem,
            this.runAsMenuItem,
            this.runAsServiceMenuItem,
            this.showDetailsForAllProcessesMenuItem,
            this.uacSeparatorMenuItem,
            this.saveMenuItem,
            this.findHandlesMenuItem,
            this.inspectPEFileMenuItem,
            this.reloadStructsMenuItem,
            this.optionsMenuItem,
            this.menuItem2,
            this.shutdownMenuItem,
            this.exitMenuItem});
            this.hackerMenuItem.Text = "&Hacker";
            // 
            // runMenuItem
            // 
            this.runMenuItem.Index = 0;
            this.runMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
            this.runMenuItem.Text = "&Run...";
            this.runMenuItem.Click += new System.EventHandler(this.runMenuItem_Click);
            // 
            // runAsAdministratorMenuItem
            // 
            this.runAsAdministratorMenuItem.Index = 1;
            this.runAsAdministratorMenuItem.Text = "Run As Administrator...";
            this.runAsAdministratorMenuItem.Click += new System.EventHandler(this.runAsAdministratorMenuItem_Click);
            // 
            // runAsMenuItem
            // 
            this.runAsMenuItem.Index = 2;
            this.runAsMenuItem.Text = "Run As...";
            this.runAsMenuItem.Visible = false;
            this.runAsMenuItem.Click += new System.EventHandler(this.runAsMenuItem_Click);
            // 
            // runAsServiceMenuItem
            // 
            this.runAsServiceMenuItem.Index = 3;
            this.runAsServiceMenuItem.Text = "Run As...";
            this.runAsServiceMenuItem.Click += new System.EventHandler(this.runAsServiceMenuItem_Click);
            // 
            // showDetailsForAllProcessesMenuItem
            // 
            this.showDetailsForAllProcessesMenuItem.Index = 4;
            this.showDetailsForAllProcessesMenuItem.Text = "Show Details for All Processes";
            this.showDetailsForAllProcessesMenuItem.Click += new System.EventHandler(this.showDetailsForAllProcessesMenuItem_Click);
            // 
            // uacSeparatorMenuItem
            // 
            this.uacSeparatorMenuItem.Index = 5;
            this.uacSeparatorMenuItem.Text = "-";
            // 
            // saveMenuItem
            // 
            this.vistaMenu.SetImage(this.saveMenuItem, global::ProcessHacker.Properties.Resources.disk);
            this.saveMenuItem.Index = 6;
            this.saveMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.saveMenuItem.Text = "Save...";
            this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
            // 
            // findHandlesMenuItem
            // 
            this.vistaMenu.SetImage(this.findHandlesMenuItem, global::ProcessHacker.Properties.Resources.find);
            this.findHandlesMenuItem.Index = 7;
            this.findHandlesMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
            this.findHandlesMenuItem.Text = "&Find Handles or DLLs...";
            this.findHandlesMenuItem.Click += new System.EventHandler(this.findHandlesMenuItem_Click);
            // 
            // inspectPEFileMenuItem
            // 
            this.vistaMenu.SetImage(this.inspectPEFileMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.inspectPEFileMenuItem.Index = 8;
            this.inspectPEFileMenuItem.Text = "Inspect &PE File...";
            this.inspectPEFileMenuItem.Click += new System.EventHandler(this.inspectPEFileMenuItem_Click);
            // 
            // reloadStructsMenuItem
            // 
            this.vistaMenu.SetImage(this.reloadStructsMenuItem, global::ProcessHacker.Properties.Resources.arrow_refresh);
            this.reloadStructsMenuItem.Index = 9;
            this.reloadStructsMenuItem.Text = "Reload Struct Definitions";
            this.reloadStructsMenuItem.Click += new System.EventHandler(this.reloadStructsMenuItem_Click);
            // 
            // optionsMenuItem
            // 
            this.vistaMenu.SetImage(this.optionsMenuItem, global::ProcessHacker.Properties.Resources.page_gear);
            this.optionsMenuItem.Index = 10;
            this.optionsMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.optionsMenuItem.Text = "&Options...";
            this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 11;
            this.menuItem2.Text = "-";
            // 
            // shutdownMenuItem
            // 
            this.shutdownMenuItem.Index = 12;
            this.shutdownMenuItem.Text = "Shutdown";
            // 
            // exitMenuItem
            // 
            this.vistaMenu.SetImage(this.exitMenuItem, global::ProcessHacker.Properties.Resources.door_out);
            this.exitMenuItem.Index = 13;
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // viewMenuItem
            // 
            this.viewMenuItem.Index = 1;
            this.viewMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.toolbarMenuItem,
            this.sysInfoMenuItem,
            this.trayIconsMenuItem,
            this.menuItem3,
            this.updateNowMenuItem,
            this.updateProcessesMenuItem,
            this.updateServicesMenuItem});
            this.viewMenuItem.Text = "&View";
            // 
            // toolbarMenuItem
            // 
            this.toolbarMenuItem.Index = 0;
            this.toolbarMenuItem.Text = "Toolbar";
            this.toolbarMenuItem.Click += new System.EventHandler(this.toolbarMenuItem_Click);
            // 
            // sysInfoMenuItem
            // 
            this.sysInfoMenuItem.Index = 1;
            this.sysInfoMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
            this.sysInfoMenuItem.Text = "System &Information";
            this.sysInfoMenuItem.Click += new System.EventHandler(this.sysInfoMenuItem_Click);
            // 
            // trayIconsMenuItem
            // 
            this.trayIconsMenuItem.Index = 2;
            this.trayIconsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.cpuHistoryMenuItem,
            this.cpuUsageMenuItem,
            this.ioHistoryMenuItem,
            this.commitHistoryMenuItem,
            this.physMemHistoryMenuItem});
            this.trayIconsMenuItem.Text = "Tray Icons";
            // 
            // cpuHistoryMenuItem
            // 
            this.cpuHistoryMenuItem.Index = 0;
            this.cpuHistoryMenuItem.Text = "CPU History";
            this.cpuHistoryMenuItem.Click += new System.EventHandler(this.cpuHistoryMenuItem_Click);
            // 
            // cpuUsageMenuItem
            // 
            this.cpuUsageMenuItem.Index = 1;
            this.cpuUsageMenuItem.Text = "CPU Usage";
            this.cpuUsageMenuItem.Click += new System.EventHandler(this.cpuUsageMenuItem_Click);
            // 
            // ioHistoryMenuItem
            // 
            this.ioHistoryMenuItem.Index = 2;
            this.ioHistoryMenuItem.Text = "I/O History";
            this.ioHistoryMenuItem.Click += new System.EventHandler(this.ioHistoryMenuItem_Click);
            // 
            // commitHistoryMenuItem
            // 
            this.commitHistoryMenuItem.Index = 3;
            this.commitHistoryMenuItem.Text = "Commit History";
            this.commitHistoryMenuItem.Click += new System.EventHandler(this.commitHistoryMenuItem_Click);
            // 
            // physMemHistoryMenuItem
            // 
            this.physMemHistoryMenuItem.Index = 4;
            this.physMemHistoryMenuItem.Text = "Physical Memory History";
            this.physMemHistoryMenuItem.Click += new System.EventHandler(this.physMemHistoryMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 3;
            this.menuItem3.Text = "-";
            // 
            // updateNowMenuItem
            // 
            this.vistaMenu.SetImage(this.updateNowMenuItem, global::ProcessHacker.Properties.Resources.arrow_refresh);
            this.updateNowMenuItem.Index = 4;
            this.updateNowMenuItem.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.updateNowMenuItem.Text = "&Refresh";
            this.updateNowMenuItem.Click += new System.EventHandler(this.updateNowMenuItem_Click);
            // 
            // updateProcessesMenuItem
            // 
            this.updateProcessesMenuItem.Index = 5;
            this.updateProcessesMenuItem.Text = "Update &Processes";
            this.updateProcessesMenuItem.Click += new System.EventHandler(this.updateProcessesMenuItem_Click);
            // 
            // updateServicesMenuItem
            // 
            this.updateServicesMenuItem.Index = 6;
            this.updateServicesMenuItem.Text = "Update &Services";
            this.updateServicesMenuItem.Click += new System.EventHandler(this.updateServicesMenuItem_Click);
            // 
            // toolsMenuItem
            // 
            this.toolsMenuItem.Index = 2;
            this.toolsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.createServiceMenuItem,
            this.hiddenProcessesMenuItem,
            this.verifyFileSignatureMenuItem});
            this.toolsMenuItem.Text = "&Tools";
            // 
            // createServiceMenuItem
            // 
            this.createServiceMenuItem.Index = 0;
            this.createServiceMenuItem.Text = "Create &Service...";
            this.createServiceMenuItem.Click += new System.EventHandler(this.createServiceMenuItem_Click);
            // 
            // hiddenProcessesMenuItem
            // 
            this.hiddenProcessesMenuItem.Index = 1;
            this.hiddenProcessesMenuItem.Text = "&Hidden Processes";
            this.hiddenProcessesMenuItem.Click += new System.EventHandler(this.hiddenProcessesMenuItem_Click);
            // 
            // verifyFileSignatureMenuItem
            // 
            this.verifyFileSignatureMenuItem.Index = 2;
            this.verifyFileSignatureMenuItem.Text = "&Verify File Signature...";
            this.verifyFileSignatureMenuItem.Click += new System.EventHandler(this.verifyFileSignatureMenuItem_Click);
            // 
            // usersMenuItem
            // 
            this.usersMenuItem.Index = 3;
            this.usersMenuItem.Text = "&Users";
            // 
            // windowMenuItem
            // 
            this.windowMenuItem.Index = 4;
            this.windowMenuItem.Text = "&Window";
            // 
            // helpMenu
            // 
            this.helpMenu.Index = 5;
            this.helpMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.freeMemoryMenuItem,
            this.checkForUpdatesMenuItem,
            this.menuItem1,
            this.logMenuItem,
            this.helpMenuItem,
            this.aboutMenuItem});
            this.helpMenu.Text = "H&elp";
            // 
            // freeMemoryMenuItem
            // 
            this.freeMemoryMenuItem.Index = 0;
            this.freeMemoryMenuItem.Text = "Free Memory";
            this.freeMemoryMenuItem.Click += new System.EventHandler(this.freeMemoryMenuItem_Click);
            // 
            // checkForUpdatesMenuItem
            // 
            this.checkForUpdatesMenuItem.Index = 1;
            this.checkForUpdatesMenuItem.Text = "Check for Updates";
            this.checkForUpdatesMenuItem.Click += new System.EventHandler(this.checkForUpdatesMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            this.menuItem1.Text = "-";
            // 
            // logMenuItem
            // 
            this.vistaMenu.SetImage(this.logMenuItem, global::ProcessHacker.Properties.Resources.page_white_text);
            this.logMenuItem.Index = 3;
            this.logMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
            this.logMenuItem.Text = "&Log";
            this.logMenuItem.Click += new System.EventHandler(this.logMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.vistaMenu.SetImage(this.helpMenuItem, global::ProcessHacker.Properties.Resources.help);
            this.helpMenuItem.Index = 4;
            this.helpMenuItem.Shortcut = System.Windows.Forms.Shortcut.F1;
            this.helpMenuItem.Text = "&Help";
            this.helpMenuItem.Click += new System.EventHandler(this.helpMenuItem_Click);
            // 
            // aboutMenuItem
            // 
            this.vistaMenu.SetImage(this.aboutMenuItem, global::ProcessHacker.Properties.Resources.information);
            this.aboutMenuItem.Index = 5;
            this.aboutMenuItem.Text = "&About";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // menuService
            // 
            this.menuService.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.goToProcessServiceMenuItem,
            this.startServiceMenuItem,
            this.continueServiceMenuItem,
            this.pauseServiceMenuItem,
            this.stopServiceMenuItem,
            this.deleteServiceMenuItem,
            this.propertiesServiceMenuItem,
            this.menuItem8,
            this.copyServiceMenuItem,
            this.selectAllServiceMenuItem});
            this.menuService.Popup += new System.EventHandler(this.menuService_Popup);
            // 
            // goToProcessServiceMenuItem
            // 
            this.vistaMenu.SetImage(this.goToProcessServiceMenuItem, global::ProcessHacker.Properties.Resources.arrow_right);
            this.goToProcessServiceMenuItem.Index = 0;
            this.goToProcessServiceMenuItem.Text = "&Go to Process";
            this.goToProcessServiceMenuItem.Click += new System.EventHandler(this.goToProcessServiceMenuItem_Click);
            // 
            // startServiceMenuItem
            // 
            this.vistaMenu.SetImage(this.startServiceMenuItem, global::ProcessHacker.Properties.Resources.control_play_blue);
            this.startServiceMenuItem.Index = 1;
            this.startServiceMenuItem.Text = "&Start";
            this.startServiceMenuItem.Click += new System.EventHandler(this.startServiceMenuItem_Click);
            // 
            // continueServiceMenuItem
            // 
            this.continueServiceMenuItem.Index = 2;
            this.continueServiceMenuItem.Text = "&Continue";
            this.continueServiceMenuItem.Click += new System.EventHandler(this.continueServiceMenuItem_Click);
            // 
            // pauseServiceMenuItem
            // 
            this.vistaMenu.SetImage(this.pauseServiceMenuItem, global::ProcessHacker.Properties.Resources.control_pause_blue);
            this.pauseServiceMenuItem.Index = 3;
            this.pauseServiceMenuItem.Text = "&Pause";
            this.pauseServiceMenuItem.Click += new System.EventHandler(this.pauseServiceMenuItem_Click);
            // 
            // stopServiceMenuItem
            // 
            this.vistaMenu.SetImage(this.stopServiceMenuItem, global::ProcessHacker.Properties.Resources.control_stop_blue);
            this.stopServiceMenuItem.Index = 4;
            this.stopServiceMenuItem.Text = "S&top";
            this.stopServiceMenuItem.Click += new System.EventHandler(this.stopServiceMenuItem_Click);
            // 
            // deleteServiceMenuItem
            // 
            this.vistaMenu.SetImage(this.deleteServiceMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.deleteServiceMenuItem.Index = 5;
            this.deleteServiceMenuItem.Shortcut = System.Windows.Forms.Shortcut.Del;
            this.deleteServiceMenuItem.Text = "Delete";
            this.deleteServiceMenuItem.Click += new System.EventHandler(this.deleteServiceMenuItem_Click);
            // 
            // propertiesServiceMenuItem
            // 
            this.propertiesServiceMenuItem.DefaultItem = true;
            this.vistaMenu.SetImage(this.propertiesServiceMenuItem, global::ProcessHacker.Properties.Resources.application_form_magnify);
            this.propertiesServiceMenuItem.Index = 6;
            this.propertiesServiceMenuItem.Text = "&Properties";
            this.propertiesServiceMenuItem.Click += new System.EventHandler(this.propertiesServiceMenuItem_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 7;
            this.menuItem8.Text = "-";
            // 
            // copyServiceMenuItem
            // 
            this.vistaMenu.SetImage(this.copyServiceMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyServiceMenuItem.Index = 8;
            this.copyServiceMenuItem.Text = "Copy";
            // 
            // selectAllServiceMenuItem
            // 
            this.selectAllServiceMenuItem.Index = 9;
            this.selectAllServiceMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
            this.selectAllServiceMenuItem.Text = "Select &All";
            this.selectAllServiceMenuItem.Click += new System.EventHandler(this.selectAllServiceMenuItem_Click);
            // 
            // menuIcon
            // 
            this.menuIcon.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.showHideMenuItem,
            this.sysInformationIconMenuItem,
            this.notificationsMenuItem,
            this.processesMenuItem,
            this.shutdownTrayMenuItem,
            this.exitTrayMenuItem});
            this.menuIcon.Popup += new System.EventHandler(this.menuIcon_Popup);
            // 
            // showHideMenuItem
            // 
            this.showHideMenuItem.Index = 0;
            this.showHideMenuItem.Text = "&Show/Hide Process Hacker";
            this.showHideMenuItem.Click += new System.EventHandler(this.showHideMenuItem_Click);
            // 
            // sysInformationIconMenuItem
            // 
            this.sysInformationIconMenuItem.Index = 1;
            this.sysInformationIconMenuItem.Text = "System &Information";
            this.sysInformationIconMenuItem.Click += new System.EventHandler(this.sysInformationIconMenuItem_Click);
            // 
            // notificationsMenuItem
            // 
            this.notificationsMenuItem.Index = 2;
            this.notificationsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.enableAllNotificationsMenuItem,
            this.disableAllNotificationsMenuItem,
            this.menuItem4,
            this.NPMenuItem,
            this.TPMenuItem,
            this.NSMenuItem,
            this.startedSMenuItem,
            this.stoppedSMenuItem,
            this.DSMenuItem});
            this.notificationsMenuItem.Text = "&Notifications";
            // 
            // enableAllNotificationsMenuItem
            // 
            this.enableAllNotificationsMenuItem.Index = 0;
            this.enableAllNotificationsMenuItem.Text = "&Enable All";
            this.enableAllNotificationsMenuItem.Click += new System.EventHandler(this.enableAllNotificationsMenuItem_Click);
            // 
            // disableAllNotificationsMenuItem
            // 
            this.disableAllNotificationsMenuItem.Index = 1;
            this.disableAllNotificationsMenuItem.Text = "&Disable All";
            this.disableAllNotificationsMenuItem.Click += new System.EventHandler(this.disableAllNotificationsMenuItem_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            this.menuItem4.Text = "-";
            // 
            // NPMenuItem
            // 
            this.NPMenuItem.Index = 3;
            this.NPMenuItem.Text = "New Processes";
            // 
            // TPMenuItem
            // 
            this.TPMenuItem.Index = 4;
            this.TPMenuItem.Text = "Terminated Processes";
            // 
            // NSMenuItem
            // 
            this.NSMenuItem.Index = 5;
            this.NSMenuItem.Text = "New Services";
            // 
            // startedSMenuItem
            // 
            this.startedSMenuItem.Index = 6;
            this.startedSMenuItem.Text = "Started Services";
            // 
            // stoppedSMenuItem
            // 
            this.stoppedSMenuItem.Index = 7;
            this.stoppedSMenuItem.Text = "Stopped Services";
            // 
            // DSMenuItem
            // 
            this.DSMenuItem.Index = 8;
            this.DSMenuItem.Text = "Deleted Services";
            // 
            // processesMenuItem
            // 
            this.processesMenuItem.Index = 3;
            this.processesMenuItem.Text = "&Processes";
            // 
            // shutdownTrayMenuItem
            // 
            this.shutdownTrayMenuItem.Index = 4;
            this.shutdownTrayMenuItem.Text = "Shutdown";
            // 
            // exitTrayMenuItem
            // 
            // this.vistaMenu.SetImage(this.exitTrayMenuItem, global::ProcessHacker.Properties.Resources.door_out);
            this.exitTrayMenuItem.Index = 5;
            this.exitTrayMenuItem.Text = "E&xit";
            this.exitTrayMenuItem.Click += new System.EventHandler(this.exitTrayMenuItem_Click);
            // 
            // goToProcessNetworkMenuItem
            // 
            this.goToProcessNetworkMenuItem.DefaultItem = true;
            //this.vistaMenu.SetImage(this.goToProcessNetworkMenuItem, global::ProcessHacker.Properties.Resources.arrow_right);
            this.goToProcessNetworkMenuItem.Index = 0;
            this.goToProcessNetworkMenuItem.Text = "&Go to Process";
            this.goToProcessNetworkMenuItem.Click += new System.EventHandler(this.goToProcessNetworkMenuItem_Click);
            // 
            // copyNetworkMenuItem
            // 
            //this.vistaMenu.SetImage(this.copyNetworkMenuItem, global::ProcessHacker.Properties.Resources.page_copy);
            this.copyNetworkMenuItem.Index = 4;
            this.copyNetworkMenuItem.Text = "&Copy";
            // 
            // closeNetworkMenuItem
            // 
            //this.vistaMenu.SetImage(this.closeNetworkMenuItem, global::ProcessHacker.Properties.Resources.cross);
            this.closeNetworkMenuItem.Index = 2;
            this.closeNetworkMenuItem.Text = "Close";
            this.closeNetworkMenuItem.Click += new System.EventHandler(this.closeNetworkMenuItem_Click);
            // 
            // menuNetwork
            // 
            this.menuNetwork.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.goToProcessNetworkMenuItem,
            this.toolsNetworkMenuItem,
            this.closeNetworkMenuItem,
            this.menuItem6,
            this.copyNetworkMenuItem,
            this.selectAllNetworkMenuItem});
            this.menuNetwork.Popup += new System.EventHandler(this.menuNetwork_Popup);
            // 
            // toolsNetworkMenuItem
            // 
            this.toolsNetworkMenuItem.Index = 1;
            this.toolsNetworkMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.whoisNetworkMenuItem,
            this.tracertNetworkMenuItem,
            this.pingNetworkMenuItem});
            this.toolsNetworkMenuItem.Text = "Tools";
            // 
            // whoisNetworkMenuItem
            // 
            this.whoisNetworkMenuItem.Index = 0;
            this.whoisNetworkMenuItem.Text = "Whois";
            this.whoisNetworkMenuItem.Click += new System.EventHandler(this.whoisNetworkMenuItem_Click);
            // 
            // tracertNetworkMenuItem
            // 
            this.tracertNetworkMenuItem.Index = 1;
            this.tracertNetworkMenuItem.Text = "Tracert";
            this.tracertNetworkMenuItem.Click += new System.EventHandler(this.tracertNetworkMenuItem_Click);
            // 
            // pingNetworkMenuItem
            // 
            this.pingNetworkMenuItem.Index = 2;
            this.pingNetworkMenuItem.Text = "Ping";
            this.pingNetworkMenuItem.Click += new System.EventHandler(this.pingNetworkMenuItem_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 3;
            this.menuItem6.Text = "-";
            // 
            // selectAllNetworkMenuItem
            // 
            this.selectAllNetworkMenuItem.Index = 5;
            this.selectAllNetworkMenuItem.Text = "Select &All";
            this.selectAllNetworkMenuItem.Click += new System.EventHandler(this.selectAllNetworkMenuItem_Click);
        }

        #region Variables

        // One-instance windows.
        public HelpWindow HelpWindow;
        public HandleFilterWindow HandleFilterWindow;
        public HiddenProcessesWindow HiddenProcessesWindow;
        public LogWindow LogWindow;
        public MiniSysInfo MiniSysInfoWindow; // Not used (yet)

        /// <summary>
        /// The thread for the System Information window. This is to avoid 
        /// freezing the main window every second to update the graphs.
        /// </summary>
        Thread sysInfoThread;
        /// <summary>
        /// The System Information window. No methods should be called on 
        /// it directly because it belongs to another thread.
        /// </summary>
        public SysInfoWindow SysInfoWindow;
        // The three main providers. They should be accessed using 
        // Program.ProcessProvider, ServiceProvider and NetworkProvider, 
        // rsepectively. However, these three variables are remnants of 
        // the old PH.
        /// <summary>
        /// The processes/system provider.
        /// </summary>
        ProcessSystemProvider processP;
        /// <summary>
        /// The services provider.
        /// </summary>
        ServiceProvider serviceP;
        /// <summary>
        /// The network connections provider.
        /// </summary>
        NetworkProvider networkP;

        /// <summary>
        /// The UAC shield bitmap. Used for the various menu items which 
        /// require UAC elevation.
        /// </summary>
        Bitmap uacShieldIcon;
        /// <summary>
        /// A black icon which all notification icons are set to initially 
        /// before their first paint.
        /// </summary>
        Icon blackIcon;
        /// <summary>
        /// A dummy UsageIcon to avoid null instance checks in the icon-related 
        /// functions.
        /// </summary>
        UsageIcon dummyIcon;
        /// <summary>
        /// The list of notification icons.
        /// </summary>
        List<UsageIcon> notifyIcons = new List<UsageIcon>();
        /// <summary>
        /// The CPU history icon, with a history of CPU usage.
        /// </summary>
        CpuHistoryIcon cpuHistoryIcon;
        /// <summary>
        /// The CPU usage icon, which indicates the current CPU usage (no history). 
        /// Dedicated to those Process Explorer users who don't like the 
        /// CPU history icon.
        /// </summary>
        CpuUsageIcon cpuUsageIcon;
        /// <summary>
        /// The I/O history icon.
        /// </summary>
        IoHistoryIcon ioHistoryIcon;
        /// <summary>
        /// The commit history icon.
        /// </summary>
        CommitHistoryIcon commitHistoryIcon;
        /// <summary>
        /// The physical memory history icon.
        /// </summary>
        PhysMemHistoryIcon physMemHistoryIcon;

        /// <summary>
        /// A dictionary relating services to processes. Each key is a PID and 
        /// each value is a list of service names hosted in that particular process.
        /// </summary>
        Dictionary<int, List<string>> processServices = new Dictionary<int, List<string>>();

        /// <summary>
        /// The number of selected processes. Not used.
        /// </summary>
        int processSelectedItems;
        /// <summary>
        /// The selected PID.
        /// </summary>
        int processSelectedPid = -1;

        /// <summary>
        /// A queue of status messages, processed by the message timer.
        /// </summary>
        Queue<KeyValuePair<string, Icon>> statusMessages = new Queue<KeyValuePair<string, Icon>>();
        /// <summary>
        /// The PH log, with events such as process creation/termination and various 
        /// service events.
        /// </summary>
        List<KeyValuePair<DateTime, string>> _log = new List<KeyValuePair<DateTime, string>>();

        /// <summary>
        /// windowhandle owned by the currently selected process. 
        /// Only populated when the user right-clicks exactly one process.
        /// </summary>
        WindowHandle windowHandle = WindowHandle.Zero;

        #endregion

        #region Properties

        // The following two properties were used by the Window menu system. 
        // Not very useful, but still needed for now.

        public MenuItem WindowMenuItem
        {
            get { return windowMenuItem; }
        }

        public wyDay.Controls.VistaMenu VistaMenu
        {
            get { return vistaMenu; }
        }

        // Mostly used by Save.cs.
        public ProcessTree ProcessTree
        {
            get { return treeProcesses; }
        }

        public int SelectedPid
        {
            get { return processSelectedPid; }
        }

        // The two properties below aren't used at all.

        public ListView ServiceList
        {
            get { return listServices.List; }
        }

        public ListView NetworkList
        {
            get { return listNetwork.List; }
        }

        /// <summary>
        /// Provides a list of service names hosted by a process.
        /// </summary>
        public IDictionary<int, List<string>> ProcessServices
        {
            get { return processServices; }
        }

        /// <summary>
        /// The PH log.
        /// </summary>
        public IList<KeyValuePair<DateTime, string>> Log
        {
            get { return _log; }
        }

        #endregion

        #region Events

        public event LogUpdatedEventHandler LogUpdated;

        #endregion

        #region Event Handlers

        #region Lists

        private void listNetwork_DoubleClick(object sender, EventArgs e)
        {
            goToProcessNetworkMenuItem_Click(sender, e);
        }

        private void listNetwork_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                goToProcessNetworkMenuItem_Click(null, null);
            }
        }

        private void listServices_DoubleClick(object sender, EventArgs e)
        {
            propertiesServiceMenuItem_Click(null, null);
        }

        private void listServices_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteServiceMenuItem_Click(null, null);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                propertiesServiceMenuItem_Click(null, null);
            }
        }

        #endregion

        #region Main Menu

        private void runMenuItem_Click(object sender, EventArgs e)
        {
            Win32.RunFileDlg(this.Handle, IntPtr.Zero, null, null, null, 0);
        }

        private void runAsMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void runAsAdministratorMenuItem_Click(object sender, EventArgs e)
        {
            PromptBox box = new PromptBox();

            box.Text = "Enter the command to start";
            box.TextBox.AutoCompleteSource = AutoCompleteSource.AllSystemSources;
            box.TextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            if (box.ShowDialog() == DialogResult.OK)
            {
                Program.StartProgramAdmin(box.Value, "", null, ShowWindowType.Show, this.Handle);
            }
        }

        private void runAsServiceMenuItem_Click(object sender, EventArgs e)
        {
            RunWindow run = new RunWindow();

            run.TopMost = this.TopMost;
            run.ShowDialog();
        }

        private void showDetailsForAllProcessesMenuItem_Click(object sender, EventArgs e)
        {
            Program.StartProcessHackerAdmin("-v", () =>
                {
                    this.Exit();
                }, this.Handle);
        }

        private void findHandlesMenuItem_Click(object sender, EventArgs e)
        {
            if (HandleFilterWindow == null)
                HandleFilterWindow = new HandleFilterWindow();

            HandleFilterWindow.Show();
            HandleFilterWindow.Activate();
        }

        private void inspectPEFileMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PEWindow pw = Program.GetPEWindow(ofd.FileName, new Program.PEWindowInvokeAction(delegate(PEWindow f)
                {
                    try
                    {
                        f.Show();
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }
                }));
            }
        }

        private void reloadStructsMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Program.Structs.Clear();
                Structs.StructParser parser = new ProcessHacker.Structs.StructParser(Program.Structs);

                parser.Parse(Application.StartupPath + "\\structs.txt");
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to load structs", ex);
            }
        }

        private void sysInfoMenuItem_Click(object sender, EventArgs e)
        {
            if (sysInfoThread == null || !sysInfoThread.IsAlive)
            {
                sysInfoThread = new Thread(() =>
                {
                    SysInfoWindow = new SysInfoWindow();

                    Application.Run(SysInfoWindow);
                });
                sysInfoThread.Start();
            }
            else
            {
                SysInfoWindow.BeginInvoke(new MethodInvoker(delegate
                {
                    SysInfoWindow.Show();
                    SysInfoWindow.Activate();
                }));
            }
        }

        private void logMenuItem_Click(object sender, EventArgs e)
        {
            if (LogWindow == null || LogWindow.IsDisposed)
            {
                LogWindow = new LogWindow();
            }

            LogWindow.Show();

            if (LogWindow.WindowState == FormWindowState.Minimized)
                LogWindow.WindowState = FormWindowState.Normal;

            LogWindow.Activate();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow about = new AboutWindow();

            about.TopMost = this.TopMost;
            about.ShowDialog();
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            OptionsWindow options = new OptionsWindow();

            options.TopMost = this.TopMost;
            DialogResult result = options.ShowDialog();

           if (result == DialogResult.OK)
           {
               this.LoadOtherSettings();
           }
        }

        private void freeMemoryMenuItem_Click(object sender, EventArgs e)
        {
            Program.CollectGarbage();
        }

        private void helpMenuItem_Click(object sender, EventArgs e)
        {
            if (HelpWindow == null)
                HelpWindow = new HelpWindow();

            HelpWindow.Show();
            HelpWindow.Activate();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Exit();
        }

        private void toolbarMenuItem_Click(object sender, EventArgs e)
        {
            toolbarMenuItem.Checked = !toolbarMenuItem.Checked;
            toolStrip.Visible = toolbarMenuItem.Checked;

            Properties.Settings.Default.ToolbarVisible = toolStrip.Visible;
            Properties.Settings.Default.Save();
        }

        private void updateNowMenuItem_Click(object sender, EventArgs e)
        {
            if (processP.RunCount > 1)
                processP.RunOnce();

            if (serviceP.RunCount > 1)
                serviceP.RunOnce();
        }

        private void updateProcessesMenuItem_Click(object sender, EventArgs e)
        {
            updateProcessesMenuItem.Checked = !updateProcessesMenuItem.Checked;
            processP.Enabled = updateProcessesMenuItem.Checked;
        }

        private void updateServicesMenuItem_Click(object sender, EventArgs e)
        {
            updateServicesMenuItem.Checked = !updateServicesMenuItem.Checked;
            serviceP.Enabled = updateServicesMenuItem.Checked;
        }

        private void hiddenProcessesMenuItem_Click(object sender, EventArgs e)
        {
            if (HiddenProcessesWindow == null || HiddenProcessesWindow.IsDisposed)
                HiddenProcessesWindow = new HiddenProcessesWindow();

            HiddenProcessesWindow.Show();

            if (HiddenProcessesWindow.WindowState == FormWindowState.Minimized)
                HiddenProcessesWindow.WindowState = FormWindowState.Normal;

            HiddenProcessesWindow.Activate();
        }

        private void verifyFileSignatureMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Filter = "Executable files (*.exe;*.dll;*.sys;*.scr;*.cpl)|*.exe;*.dll;*.sys;*.scr;*.cpl|All files (*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var result = Cryptography.VerifyFile(ofd.FileName);
                    string message = "";

                    switch (result)
                    {
                        case VerifyResult.Distrust:
                            message = "is not trusted";
                            break;
                        case VerifyResult.Expired:
                            message = "has an expired certificate";
                            break;
                        case VerifyResult.NoSignature:
                            message = "does not have a digital signature";
                            break;
                        case VerifyResult.Revoked:
                            message = "has a revoked certificate";
                            break;
                        case VerifyResult.SecuritySettings:
                            message = "could not be verified due to security settings";
                            break;
                        case VerifyResult.Trusted:
                            message = "is trusted";
                            break;
                        case VerifyResult.Unknown:
                            message = "could not be verified";
                            break;
                        default:
                            message = "could not be verified";
                            break;
                    }

                    PhUtils.ShowInformation("The file \"" + ofd.FileName + "\" " + message + ".");
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to verify the file", ex);
                }
            }
        }

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            Save.SaveToFile();
        }

        private void createServiceMenuItem_Click(object sender, EventArgs e)
        {
            CreateServiceWindow createServiceWindow = new CreateServiceWindow();

            createServiceWindow.TopMost = this.TopMost;
            createServiceWindow.ShowDialog();
        }

        private void checkForUpdatesMenuItem_Click(object sender, EventArgs e)
        {
            this.UpdateProgram(true);
        }

        #region View

        private void cpuHistoryMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CpuHistoryIconVisible =
                cpuHistoryMenuItem.Checked = !cpuHistoryMenuItem.Checked;
            this.ApplyIconVisibilities();
        }

        private void cpuUsageMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CpuUsageIconVisible =
                cpuUsageMenuItem.Checked = !cpuUsageMenuItem.Checked;
            this.ApplyIconVisibilities();
        }

        private void ioHistoryMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.IoHistoryIconVisible =
                ioHistoryMenuItem.Checked = !ioHistoryMenuItem.Checked;
            this.ApplyIconVisibilities();
        }

        private void commitHistoryMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CommitHistoryIconVisible =
             commitHistoryMenuItem.Checked = !commitHistoryMenuItem.Checked;
            this.ApplyIconVisibilities();
        }

        private void physMemHistoryMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.PhysMemHistoryIconVisible =
               physMemHistoryMenuItem.Checked = !physMemHistoryMenuItem.Checked;
            this.ApplyIconVisibilities();
        }

        #endregion

        #endregion

        #region Network Context Menu

        private void menuNetwork_Popup(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count == 0)
            {
                menuNetwork.DisableAll();
            }
            else if (listNetwork.SelectedItems.Count == 1)
            {
                menuNetwork.EnableAll();
            }
            else
            {
                menuNetwork.EnableAll();
                goToProcessNetworkMenuItem.Enabled = false;
            }

            if (listNetwork.Items.Count > 0)
                selectAllNetworkMenuItem.Enabled = true;
            else
                selectAllNetworkMenuItem.Enabled = false;

            try
            {
                bool hasValid = false;

                foreach (ListViewItem item in listNetwork.SelectedItems)
                {
                    if (item.SubItems[5].Text == "TCP")
                    {
                        if (item.SubItems[6].Text != "Listening" &&
                            item.SubItems[6].Text != "CloseWait" &&
                            item.SubItems[6].Text != "TimeWait")
                        {
                            hasValid = true;
                            break;
                        }
                    }
                }

                if (!hasValid)
                    closeNetworkMenuItem.Enabled = false;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            try
            {
                bool hasValid = false;

                foreach (ListViewItem item in listNetwork.SelectedItems)
                {
                    if (item.SubItems[7].Text.Length > 0)
                    {
                        hasValid = true; 
                        break;
                    }
                }

                if (!hasValid)
                {
                    whoisNetworkMenuItem.Enabled = false;
                    tracertNetworkMenuItem.Enabled = false;
                    pingNetworkMenuItem.Enabled = false;
                }
                else
                {
                    whoisNetworkMenuItem.Enabled = true;
                    tracertNetworkMenuItem.Enabled = true;
                    pingNetworkMenuItem.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private void goToProcessNetworkMenuItem_Click(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count != 1)
                return;

            this.SelectProcess((int)listNetwork.SelectedItems[0].Tag);
        }

        private void whoisNetworkMenuItem_Click(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count != 1)
                return;

            foreach (ListViewItem item in listNetwork.SelectedItems)
            {
                string SelectedItem = item.SubItems[7].Text; 

                if (SelectedItem.Length > 0)
                {
                    IPInfoWindow iw = new IPInfoWindow(SelectedItem, IpAction.Whois);
                    iw.ShowDialog(this);
                }
            }
        }

        private void tracertNetworkMenuItem_Click(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count != 1)
                return;

            foreach (ListViewItem item in listNetwork.SelectedItems)
            {
                string SelectedItem = item.SubItems[7].Text;

                if (SelectedItem.Length > 0)
                {
                    IPInfoWindow iw = new IPInfoWindow(SelectedItem, IpAction.Tracert);
                    iw.ShowDialog(this);
                }
            }
        }

        private void pingNetworkMenuItem_Click(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count != 1)
                return;

            foreach (ListViewItem item in listNetwork.SelectedItems)
            {
                string SelectedItem = item.SubItems[7].Text;

                if (SelectedItem.Length > 0)
                {
                    IPInfoWindow iw = new IPInfoWindow(SelectedItem, IpAction.Ping);
                    iw.ShowDialog(this);
                }
            }
        }

        private void closeNetworkMenuItem_Click(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count == 0)
                return;

            bool allGood = true;

            try
            {
                foreach (ListViewItem item in listNetwork.SelectedItems)
                {
                    if (item.SubItems[5].Text != "TCP" ||
                        item.SubItems[6].Text != "Established")
                        continue;

                    try
                    {
                        networkP.Dictionary[item.Name].Connection.CloseTcpConnection();
                    }
                    catch
                    {
                        allGood = false;

                        if (MessageBox.Show("Could not close the TCP connection. " +
                            "Make sure Process Hacker is running with administrative privileges.", "Process Hacker",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            if (allGood)
            {
                foreach (ListViewItem item in listNetwork.SelectedItems)
                    item.Selected = false;
            }          

        }

        private void selectAllNetworkMenuItem_Click(object sender, EventArgs e)
        {
            Utils.SelectAll(listNetwork.List.Items);
        }

        #endregion

        #region Notification Icon & Menu

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            showHideMenuItem_Click(null, null);
        }

        private void menuIcon_Popup(object sender, EventArgs e)
        {
            List<ProcessItem> processes = new List<ProcessItem>();

            // Clear the images so we don't get GDI+ handle leaks
            foreach (MenuItem item in processesMenuItem.MenuItems)
                vistaMenu.SetImage(item, null);

            processesMenuItem.MenuItems.DisposeAndClear();

            // HACK: To be fixed later - we need some sort of locking for the process provider
            try
            {
                foreach (var process in processP.Dictionary.Values)
                {
                    if (process.Pid > 0)
                    {
                        processes.Add(process);
                    }
                }

                // Remove zero CPU usage processes and processes running as other users
                for (int i = 0; i < processes.Count && processes.Count > Properties.Settings.Default.IconMenuProcessCount; i++)
                {
                    if (processes[i].CpuUsage == 0)
                    {
                        processes.RemoveAt(i);
                        i--;
                    }
                    else if (processes[i].Username != Program.CurrentUsername)
                    {
                        processes.RemoveAt(i);
                        i--;
                    }
                }

                // Sort the processes by CPU usage and remove processes with low CPU usage
                processes.Sort((i1, i2) => -i1.CpuUsage.CompareTo(i2.CpuUsage));

                if (processes.Count > Properties.Settings.Default.IconMenuProcessCount)
                {
                    int c = processes.Count;
                    processes.RemoveRange(Properties.Settings.Default.IconMenuProcessCount,
                        processes.Count - Properties.Settings.Default.IconMenuProcessCount);
                }

                // Then sort the processes by name
                processes.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));

                // Add the processes
                foreach (var process in processes)
                {
                    MenuItem processItem = new MenuItem();
                    MenuItem terminateItem = new MenuItem();
                    MenuItem suspendItem = new MenuItem();
                    MenuItem resumeItem = new MenuItem();
                    MenuItem propertiesItem = new MenuItem();

                    processItem.Text = process.Name + " (" + process.Pid.ToString() + ")";
                    processItem.Tag = process;

                    terminateItem.Click += new EventHandler((sender_, e_) =>
                    {
                        ProcessItem item = (ProcessItem)((MenuItem)sender_).Parent.Tag;

                        ProcessActions.Terminate(this, new int[] { item.Pid }, new string[] { item.Name }, true);
                    });
                    terminateItem.Text = "Terminate";

                    suspendItem.Click += new EventHandler((sender_, e_) =>
                    {
                        ProcessItem item = (ProcessItem)((MenuItem)sender_).Parent.Tag;

                        ProcessActions.Suspend(this, new int[] { item.Pid }, new string[] { item.Name }, true);
                    });
                    suspendItem.Text = "Suspend";

                    resumeItem.Click += new EventHandler((sender_, e_) =>
                    {
                        ProcessItem item = (ProcessItem)((MenuItem)sender_).Parent.Tag;

                        ProcessActions.Resume(this, new int[] { item.Pid }, new string[] { item.Name }, true);
                    });
                    resumeItem.Text = "Resume";

                    propertiesItem.Click += new EventHandler((sender_, e_) =>
                    {
                        try
                        {
                            ProcessItem item = (ProcessItem)((MenuItem)sender_).Parent.Tag;

                            ProcessWindow pForm = Program.GetProcessWindow(processP.Dictionary[item.Pid],
                                new Program.PWindowInvokeAction(delegate(ProcessWindow f)
                            {
                                f.Show();
                                f.Activate();
                            }));
                        }
                        catch (Exception ex)
                        {
                            PhUtils.ShowException("Unable to inspect the process", ex);
                        }
                    });
                    propertiesItem.Text = "Properties";

                    processItem.MenuItems.AddRange(new MenuItem[] { terminateItem, suspendItem, resumeItem, propertiesItem });
                    processesMenuItem.MenuItems.Add(processItem);

                    vistaMenu.SetImage(processItem, (treeProcesses.Tree.Model as ProcessTreeModel).Nodes[process.Pid].Icon);
                }
            }
            catch
            {
                foreach (MenuItem item in processesMenuItem.MenuItems)
                    vistaMenu.SetImage(item, null);

                processesMenuItem.MenuItems.DisposeAndClear();
            }
        }

        private void showHideMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.Visible)
            {
                Properties.Settings.Default.WindowLocation = this.Location;
                Properties.Settings.Default.WindowSize = this.Size;
            }

            this.Visible = !this.Visible;

            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Location = Properties.Settings.Default.WindowLocation;
                this.Size = Properties.Settings.Default.WindowSize;
                this.WindowState = FormWindowState.Normal;
            }

            this.Activate();
        }

        private void sysInformationIconMenuItem_Click(object sender, EventArgs e)
        {
            sysInfoMenuItem_Click(sender, e);
        }

        private void enableAllNotificationsMenuItem_Click(object sender, EventArgs e)
        {
            NPMenuItem.Checked = true;
            TPMenuItem.Checked = true;
            NSMenuItem.Checked = true;
            startedSMenuItem.Checked = true;
            stoppedSMenuItem.Checked = true;
            DSMenuItem.Checked = true;
        }

        private void disableAllNotificationsMenuItem_Click(object sender, EventArgs e)
        {
            NPMenuItem.Checked = false;
            TPMenuItem.Checked = false;
            NSMenuItem.Checked = false;
            startedSMenuItem.Checked = false;
            stoppedSMenuItem.Checked = false;
            DSMenuItem.Checked = false;
        }

        private void exitTrayMenuItem_Click(object sender, EventArgs e)
        {
            this.Exit();
        }

        #endregion

        #region Process Context Menu

        private void menuProcess_Popup(object sender, EventArgs e)
        {
            virtualizationProcessMenuItem.Checked = false;

            // Menu item fixup...
            if (treeProcesses.SelectedTreeNodes.Count == 0)
            {
                // If nothing is selected, disable everything.
                // The Select All menu item will be enabled later if 
                // we have at least one process in the tree.
                menuProcess.DisableAll();
            }
            else if (treeProcesses.SelectedTreeNodes.Count == 1)
            {
                // All actions should work with one process selected.
                menuProcess.EnableAll();

                // Singular nouns.
                priorityMenuItem.Text = "&Priority";
                terminateMenuItem.Text = "&Terminate Process";
                suspendMenuItem.Text = "&Suspend Process";
                resumeMenuItem.Text = "&Resume Process";

                // Check the appropriate priority level menu item.
                realTimeMenuItem.Checked = false;
                highMenuItem.Checked = false;
                aboveNormalMenuItem.Checked = false;
                normalMenuItem.Checked = false;
                belowNormalMenuItem.Checked = false;
                idleMenuItem.Checked = false;

                try
                {
                    using (var phandle = new ProcessHandle(processSelectedPid, Program.MinProcessQueryRights))
                    {
                        switch (phandle.GetPriorityClass())
                        {
                            case ProcessPriorityClass.RealTime:
                                realTimeMenuItem.Checked = true;
                                break;

                            case ProcessPriorityClass.High:
                                highMenuItem.Checked = true;
                                break;

                            case ProcessPriorityClass.AboveNormal:
                                aboveNormalMenuItem.Checked = true;
                                break;

                            case ProcessPriorityClass.Normal:
                                normalMenuItem.Checked = true;
                                break;

                            case ProcessPriorityClass.BelowNormal:
                                belowNormalMenuItem.Checked = true;
                                break;

                            case ProcessPriorityClass.Idle:
                                idleMenuItem.Checked = true;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    priorityMenuItem.Text = "(" + ex.Message + ")";
                    priorityMenuItem.Enabled = false;
                }

                // Check the virtualization menu item.
                try
                {
                    using (var phandle = new ProcessHandle(processSelectedPid, Program.MinProcessQueryRights))
                    {
                        try
                        {
                            using (var thandle = phandle.GetToken(TokenAccess.Query))
                            {
                                if (virtualizationProcessMenuItem.Enabled = thandle.IsVirtualizationAllowed())
                                    virtualizationProcessMenuItem.Checked = thandle.IsVirtualizationEnabled();
                            }
                        }
                        catch
                        { }
                    }
                }
                catch
                {
                    virtualizationProcessMenuItem.Enabled = false;
                }

                // Enable/disable DLL injection based on the process' session ID.
                try
                {
                    if (processP.Dictionary[processSelectedPid].SessionId != Program.CurrentSessionId)
                        injectDllProcessMenuItem.Enabled = false;
                    else
                        injectDllProcessMenuItem.Enabled = true;
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }

                // Disable Terminate Process Tree if the selected process doesn't 
                // have any children. Note that this may also happen if the user 
                // is sorting the list (!).
                try
                {
                    if (treeProcesses.SelectedTreeNodes[0].IsLeaf &&
                        (treeProcesses.Tree.Model as ProcessTreeModel).GetSortColumn() == "")
                        terminateProcessTreeMenuItem.Visible = false;
                    else
                        terminateProcessTreeMenuItem.Visible = true;
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }

                // Find the process' window (if any).
                windowHandle = WindowHandle.Zero;
                WindowHandle.Enumerate(
                    (handle) =>
                    {
                        // GetWindowLong
                        // Shell_TrayWnd
                        if (handle.IsWindow() && handle.IsVisible() && handle.IsParent())
                        {
                            int pid;
                            Win32.GetWindowThreadProcessId(handle, out pid);

                            if (pid == processSelectedPid)
                            {
                                windowHandle = handle;
                                return false;
                            }
                        }
                        return true;
                    });

                // Enable the Window submenu if we found window owned 
                // by the process. Otherwise, disable the submenu.
                if (windowHandle.IsInvalid)
                {
                    windowProcessMenuItem.Enabled = false;
                }
                else
                {
                    windowProcessMenuItem.Enabled = true;
                    windowProcessMenuItem.EnableAll();

                    switch (windowHandle.GetPlacement().ShowState)
                    {
                        case ShowWindowType.ShowMinimized:
                            minimizeProcessMenuItem.Enabled = false;                          
                            break;

                        case ShowWindowType.ShowMaximized:
                            maximizeProcessMenuItem.Enabled = false;                       
                            break;

                        case ShowWindowType.ShowNormal:
                            restoreProcessMenuItem.Enabled = false; 
                            break;                       
                    }
                }
            }
            else
            {
                // Assume most process actions will not work with more than one process.
                menuProcess.DisableAll();

                // Use plural nouns.
                terminateMenuItem.Text = "&Terminate Processes";
                suspendMenuItem.Text = "&Suspend Processes";
                resumeMenuItem.Text = "&Resume Processes";

                // Enable a specific set of actions.
                terminateMenuItem.Enabled = true;
                suspendMenuItem.Enabled = true;
                resumeMenuItem.Enabled = true;
                reduceWorkingSetProcessMenuItem.Enabled = true;
                copyProcessMenuItem.Enabled = true;
            }

            // Special case for invalid PIDs.
            if (processSelectedPid <= 0 && treeProcesses.SelectedNodes.Count == 1)
            {
                priorityMenuItem.Text = "&Priority";
                menuProcess.DisableAll();
                propertiesProcessMenuItem.Enabled = true;
            }

            // Enable/disable the Select All menu item.
            if (treeProcesses.Model.Nodes.Count == 0)
            {
                selectAllProcessMenuItem.Enabled = false;
            }
            else
            {
                selectAllProcessMenuItem.Enabled = true;
            }
        }

        private void terminateMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count == 0)
                return;

            int[] pids = new int[treeProcesses.SelectedNodes.Count];
            string[] names = new string[pids.Length];

            for (int i = 0; i < treeProcesses.SelectedNodes.Count; i++)
            {
                pids[i] = treeProcesses.SelectedNodes[i].Pid;
                names[i] = treeProcesses.SelectedNodes[i].Name;
            }

            if (ProcessActions.Terminate(this, pids, names, true))
            {
                try
                {
                    TreeNodeAdv[] nodes = new TreeNodeAdv[treeProcesses.SelectedTreeNodes.Count];

                    treeProcesses.SelectedTreeNodes.CopyTo(nodes, 0);

                    foreach (TreeNodeAdv node in nodes)
                        node.IsSelected = false;
                }
                catch
                { }
            }
        }

        private void terminateProcessTreeMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count == 0)
                return;

            int[] pids = new int[treeProcesses.SelectedNodes.Count];
            string[] names = new string[pids.Length];

            for (int i = 0; i < treeProcesses.SelectedNodes.Count; i++)
            {
                pids[i] = treeProcesses.SelectedNodes[i].Pid;
                names[i] = treeProcesses.SelectedNodes[i].Name;
            }

            if (ProcessActions.TerminateTree(this, pids, names, true))
            {
                try
                {
                    TreeNodeAdv[] nodes = new TreeNodeAdv[treeProcesses.SelectedTreeNodes.Count];

                    treeProcesses.SelectedTreeNodes.CopyTo(nodes, 0);

                    foreach (TreeNodeAdv node in nodes)
                        node.IsSelected = false;
                }
                catch
                { }
            }
        }

        private void suspendMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count == 0)
                return;

            int[] pids = new int[treeProcesses.SelectedNodes.Count];
            string[] names = new string[pids.Length];

            for (int i = 0; i < treeProcesses.SelectedNodes.Count; i++)
            {
                pids[i] = treeProcesses.SelectedNodes[i].Pid;
                names[i] = treeProcesses.SelectedNodes[i].Name;
            }

            ProcessActions.Suspend(this, pids, names, true);
        }

        private void resumeMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count == 0)
                return;

            int[] pids = new int[treeProcesses.SelectedNodes.Count];
            string[] names = new string[pids.Length];

            for (int i = 0; i < treeProcesses.SelectedNodes.Count; i++)
            {
                pids[i] = treeProcesses.SelectedNodes[i].Pid;
                names[i] = treeProcesses.SelectedNodes[i].Name;
            }

            ProcessActions.Resume(this, pids, names, true);
        }

        private void restartProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (PhUtils.ShowConfirmMessage(
                "restart",
                "the selected process",
                "The process will be restarted with the same command line and " + 
                "working directory, but if it is running under a different user it " + 
                "will be restarted under the current user.",
                true
                ))
            {
                try
                {
                    using (var phandle = new ProcessHandle(processSelectedPid,
                        Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights))
                    {
                        string currentDirectory = phandle.GetPebString(PebOffset.CurrentDirectoryPath);
                        string cmdLine = phandle.GetPebString(PebOffset.CommandLine);

                        try
                        {
                            using (var phandle2 = new ProcessHandle(processSelectedPid, ProcessAccess.Terminate))
                                phandle2.Terminate();
                        }
                        catch (Exception ex)
                        {
                            PhUtils.ShowException("Unable to terminate the process", ex);
                            return;
                        }

                        try
                        {
                            var startupInfo = new StartupInfo();
                            var procInfo = new ProcessInformation();

                            startupInfo.Size = Marshal.SizeOf(startupInfo);

                            if (!Win32.CreateProcess(null, cmdLine, IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, currentDirectory,
                                ref startupInfo, out procInfo))
                                Win32.ThrowLastError();

                            Win32.CloseHandle(procInfo.hProcess);
                            Win32.CloseHandle(procInfo.hThread);
                        }
                        catch (Exception ex)
                        {
                            PhUtils.ShowException("Unable to start the command '" + cmdLine + "'", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to restart the process", ex);
                }
            }
        }

        private void reduceWorkingSetProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count == 0)
                return;

            int[] pids = new int[treeProcesses.SelectedNodes.Count];
            string[] names = new string[pids.Length];

            for (int i = 0; i < treeProcesses.SelectedNodes.Count; i++)
            {
                pids[i] = treeProcesses.SelectedNodes[i].Pid;
                names[i] = treeProcesses.SelectedNodes[i].Name;
            }

            ProcessActions.ReduceWorkingSet(this, pids, names, false);
        }

        private void virtualizationProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (!PhUtils.ShowConfirmMessage(
                "set",
                "virtualization for the process",
                "Enabling or disabling virtualization for a process may " + 
                "alter its functionality and produce undesirable effects.",
                false
                ))
                return;

            try
            {
                using (var phandle = new ProcessHandle(processSelectedPid, Program.MinProcessQueryRights))
                {
                    using (var thandle = phandle.GetToken(TokenAccess.GenericWrite))
                    {
                        thandle.SetVirtualizationEnabled(!virtualizationProcessMenuItem.Checked);
                    }
                }
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to set process virtualization", ex);
            }
        }

        private void propertiesProcessMenuItem_Click(object sender, EventArgs e)
        {
            // user hasn't got any processes selected
            if (processSelectedPid == -1)
                return;

            ProcessActions.ShowProperties(this, processSelectedPid, treeProcesses.SelectedNodes[0].Name);
        }

        private void affinityProcessMenuItem_Click(object sender, EventArgs e)
        {
            ProcessAffinity affForm = new ProcessAffinity(processSelectedPid);

            try
            {
                affForm.TopMost = this.TopMost;
                affForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private void createDumpFileProcessMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "Dump Files (*.dmp)|*.dmp|All Files (*.*)|*.*";
            sfd.FileName =
                processP.Dictionary[processSelectedPid].Name +
                "_" +
                DateTime.Now.ToString("yyMMdd") +
                ".dmp";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    Exception exception = null;

                    ThreadStart dumpProcess = () =>
                        {
                            try
                            {
                                using (var phandle = new ProcessHandle(processSelectedPid,
                                    ProcessAccess.DupHandle | ProcessAccess.QueryInformation |
                                    ProcessAccess.SuspendResume | ProcessAccess.VmRead))
                                    phandle.WriteDump(sfd.FileName);
                            }
                            catch (Exception ex2)
                            {
                                exception = ex2;
                            }
                        };

                    if (OSVersion.HasTaskDialogs)
                    {
                        // Use a task dialog to display a fancy progress bar.
                        TaskDialog td = new TaskDialog();
                        Thread t = new Thread(dumpProcess);

                        td.AllowDialogCancellation = false;
                        td.Buttons = new TaskDialogButton[] { new TaskDialogButton((int)DialogResult.OK, "Close") };
                        td.WindowTitle = "Process Hacker";
                        td.MainInstruction = "Creating the dump file...";
                        td.ShowMarqueeProgressBar = true;
                        td.EnableHyperlinks = true;
                        td.CallbackTimer = true;
                        td.Callback = (taskDialog, args, userData) =>
                            {
                                if (args.Notification == TaskDialogNotification.Created)
                                {
                                    taskDialog.SetMarqueeProgressBar(true);
                                    taskDialog.SetProgressBarState(ProgressBarState.Normal);
                                    taskDialog.SetProgressBarMarquee(true, 100);
                                    taskDialog.EnableButton((int)DialogResult.OK, false);
                                }
                                else if (args.Notification == TaskDialogNotification.Timer)
                                {
                                    if (!t.IsAlive)
                                    {
                                        taskDialog.EnableButton((int)DialogResult.OK, true);
                                        taskDialog.SetProgressBarMarquee(false, 0);
                                        taskDialog.SetMarqueeProgressBar(false);

                                        if (exception == null)
                                        {
                                            taskDialog.SetMainInstruction("The dump file has been created.");
                                            taskDialog.SetContent(
                                                "The dump file has been saved at: <a href=\"file\">" + sfd.FileName + "</a>.");
                                        }
                                        else
                                        {
                                            taskDialog.UpdateMainIcon(TaskDialogIcon.Warning);
                                            taskDialog.SetMainInstruction("Unable to create the dump file.");
                                            taskDialog.SetContent(
                                                "The dump file could not be created: " + exception.Message
                                                );
                                        }
                                    }
                                }
                                else if (args.Notification == TaskDialogNotification.HyperlinkClicked)
                                {
                                    if (args.Hyperlink == "file")
                                        Utils.ShowFileInExplorer(sfd.FileName);

                                    return true;
                                }

                                return false;
                            };

                        t.Start();
                        td.Show(this);
                    }
                    else
                    {
                        // No task dialogs, do the thing on the GUI thread.
                        dumpProcess();

                        if (exception != null)
                            PhUtils.ShowException("Unable to create the dump file", exception);
                    }
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to create the dump file", ex);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void terminatorProcessMenuItem_Click(object sender, EventArgs e)
        {
            TerminatorWindow w = new TerminatorWindow(processSelectedPid);

            w.Text = "Terminator - " + processP.Dictionary[processSelectedPid].Name +
                " (PID " + processSelectedPid.ToString() + ")";

            w.TopMost = this.TopMost;
            w.ShowDialog();
        }

        #region Run As

        private void launchAsUserProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.RunAsCommand = processP.Dictionary[processSelectedPid].FileName;

                RunWindow run = new RunWindow();

                run.TopMost = this.TopMost;
                run.ShowDialog();
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private void launchAsThisUserProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                RunWindow run = new RunWindow();

                run.TopMost = this.TopMost;
                run.UsePID(processSelectedPid);
                run.ShowDialog();
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        #endregion

        #region Miscellaneous

        private void detachFromDebuggerProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var phandle = new ProcessHandle(processSelectedPid, ProcessAccess.QueryInformation | ProcessAccess.SuspendResume))
                {
                    using (var dhandle = phandle.GetDebugObject())
                        phandle.RemoveDebug(dhandle);
                }
            }
            catch (WindowsException ex)
            {
                if (ex.Status == NtStatus.PortNotSet)
                    PhUtils.ShowInformation("The process is not being debugged.");
                else
                    PhUtils.ShowException("Unable to detach the process", ex);
            }
        }

        private void heapsProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                HeapsWindow heapsWindow;

                using (DebugBuffer buffer = new DebugBuffer())
                {
                    this.Cursor = Cursors.WaitCursor;

                    try
                    {
                        buffer.Query(
                            processSelectedPid,
                            RtlQueryProcessDebugFlags.HeapSummary |
                            RtlQueryProcessDebugFlags.HeapEntries
                            );
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }

                    heapsWindow = new HeapsWindow(processSelectedPid, buffer.GetHeaps());
                }

                heapsWindow.TopMost = this.TopMost;
                heapsWindow.ShowDialog();
            }
            catch (WindowsException ex)
            {
                PhUtils.ShowException("Unable to get heap information", ex);
            }
        }

        private void injectDllProcessMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "DLL Files (*.dll)|*.dll|All Files (*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var phandle = new ProcessHandle(processSelectedPid,
                        ProcessAccess.CreateThread | ProcessAccess.VmOperation | ProcessAccess.VmWrite))
                    {
                        phandle.InjectDll(ofd.FileName, 5000);
                    }
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to inject the DLL", ex);
                }
            }
        }

        private void protectionProcessMenuItem_Click(object sender, EventArgs e)
        {
            var protectProcessWindow = new ProtectProcessWindow(processSelectedPid);

            protectProcessWindow.TopMost = this.TopMost;
            protectProcessWindow.ShowDialog();
        }

        private void setTokenProcessMenuItem_Click(object sender, EventArgs e)
        {
            ProcessPickerWindow picker = new ProcessPickerWindow();

            picker.Label = "Select the source of the token:";
            picker.TopMost = this.TopMost;

            if (picker.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    KProcessHacker.Instance.SetProcessToken(picker.SelectedPid, processSelectedPid);
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to set the process token", ex);
                }
            }
        }

        #endregion

        #region Priority

        private void realTimeMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.RealTime);
        }

        private void highMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.High);
        }

        private void aboveNormalMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.AboveNormal);
        }

        private void normalMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.Normal);
        }

        private void belowNormalMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.BelowNormal);
        }

        private void idleMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.Idle);
        }

        #endregion

        #region Window

        private void bringToFrontProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (!windowHandle.IsInvalid && windowHandle.IsWindow())
            {
                WindowPlacement placement = windowHandle.GetPlacement();

                if (placement.ShowState == ShowWindowType.ShowMinimized)
                    windowHandle.Show(ShowWindowType.Restore);
                else
                    windowHandle.SetForeground();
            }
        }

        private void restoreProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (!windowHandle.IsInvalid && windowHandle.IsWindow())
            {
                windowHandle.Show(ShowWindowType.Restore);
            }
        }

        private void minimizeProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (!windowHandle.IsInvalid && windowHandle.IsWindow())
            {
                windowHandle.Show(ShowWindowType.ShowMinimized);
            }
        }

        private void maximizeProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (!windowHandle.IsInvalid && windowHandle.IsWindow())
            {
                windowHandle.Show(ShowWindowType.ShowMaximized);
            }
        }

        private void closeProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (!windowHandle.IsInvalid && windowHandle.IsWindow())
            {
                windowHandle.PostMessage(WindowMessage.Close, 0, 0);
                //windowHandle.Close();
            }
        }

        #endregion

        private void searchProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count != 1)
                return;

            try
            {
                Process.Start(Properties.Settings.Default.SearchEngine.Replace("%s",
                    treeProcesses.SelectedNodes[0].Name));
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to search for the process", ex);
            }
        }

        private void reanalyzeProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                processP.QueueProcessQuery(processSelectedPid);
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private void selectAllProcessMenuItem_Click(object sender, EventArgs e)
        {
            treeProcesses.Tree.AllNodes.SelectAll();
            treeProcesses.Tree.Invalidate();
        }

        #endregion

        #region Providers

        private void processP_Updated()
        {
            processP.DictionaryAdded += processP_DictionaryAdded;
            processP.DictionaryRemoved += processP_DictionaryRemoved;
            processP.Updated -= processP_Updated;

            try { Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High; }
            catch { }

            if (processP.RunCount >= 1)
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    treeProcesses.Tree.EndCompleteUpdate();
                    treeProcesses.Tree.EndUpdate();

                    if (Properties.Settings.Default.ScrollDownProcessTree)
                    {
                        // HACK HACK HACK HACK
                        // HACK HACK HACK HACK
                        // HACK HACK HACK HACK
                        // HACK HACK HACK HACK
                        try
                        {
                            foreach (var process in treeProcesses.Model.Roots)
                            {
                                if (
                                    string.Equals(process.Name, "explorer.exe",
                                    StringComparison.InvariantCultureIgnoreCase) &&
                                    process.ProcessItem.Username == Program.CurrentUsername)
                                {
                                    treeProcesses.FindTreeNode(process).EnsureVisible2();

                                    break;
                                }
                            }
                        }
                        catch
                        { }
                    }

                    treeProcesses.Invalidate();
                    processP.RunOnceAsync();
                    this.Cursor = Cursors.Default;
                    this.UpdateCommon();
                }));
        }

        private void processP_InfoUpdater()
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                UpdateStatusInfo();
            }));
        }

        private void processP_FileProcessingReceived(int stage, int pid)
        {
            // Check if we need to inspect a process at startup.
            if (stage == 0x1 && Program.InspectPid != -1 && pid == Program.InspectPid)
            {
                processP.ProcessQueryReceived -= processP_FileProcessingReceived;
                ProcessActions.ShowProperties(this, pid, processP.Dictionary[pid].Name);
            }
        }

        public void processP_DictionaryAdded(ProcessItem item)
        {
            ProcessItem parent = null;
            string parentText = "";

            if (item.HasParent && processP.Dictionary.ContainsKey(item.ParentPid))
            {
                try
                {
                    parent = processP.Dictionary[item.ParentPid];

                    parentText += " started by " + parent.Name + " (PID " + parent.Pid.ToString() + ")";
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }
            }

            this.QueueMessage("New Process: " + item.Name + " (PID " + item.Pid.ToString() + ")" + parentText, item.Icon);

            if (NPMenuItem.Checked)
                this.GetFirstIcon().ShowBalloonTip(2000, "New Process",
                    "The process " + item.Name + " (" + item.Pid.ToString() +
                    ") was started" + ((parentText != "") ? " by " +
                    parent.Name + " (PID " + parent.Pid.ToString() + ")" : "") + ".", ToolTipIcon.Info);
        }

        public void processP_DictionaryRemoved(ProcessItem item)
        {
            this.QueueMessage("Terminated Process: " + item.Name + " (PID " + item.Pid.ToString() + ")", null);

            if (processServices.ContainsKey(item.Pid))
                processServices.Remove(item.Pid);

            if (TPMenuItem.Checked)
                this.GetFirstIcon().ShowBalloonTip(2000, "Terminated Process",
                    "The process " + item.Name + " (" + item.Pid.ToString() + ") was terminated.", ToolTipIcon.Info);
        }

        private void serviceP_Updated()
        {
            listServices.BeginInvoke(new MethodInvoker(delegate
            {
                listServices.List.EndUpdate();
            }));

            HighlightingContext.StateHighlighting = true;

            serviceP.DictionaryAdded += serviceP_DictionaryAdded;
            serviceP.DictionaryModified += serviceP_DictionaryModified;
            serviceP.DictionaryRemoved += serviceP_DictionaryRemoved;
            serviceP.Updated -= serviceP_Updated;

            if (processP.RunCount >= 1)
                this.BeginInvoke(new MethodInvoker(UpdateCommon));
        }

        public void serviceP_DictionaryAdded(ServiceItem item)
        {
            this.QueueMessage("New Service: " + item.Status.ServiceName +
                " (" + item.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                ((item.Status.DisplayName != "") ?
                " (" + item.Status.DisplayName + ")" :
                ""), null);

            if (NSMenuItem.Checked)
                this.GetFirstIcon().ShowBalloonTip(2000, "New Service",
                    "The service " + item.Status.ServiceName + " (" + item.Status.DisplayName + ") has been created.",
                    ToolTipIcon.Info);
        }

        public void serviceP_DictionaryAdded_Process(ServiceItem item)
        {
            if (item.Status.ServiceStatusProcess.ProcessID != 0)
            {
                if (!processServices.ContainsKey(item.Status.ServiceStatusProcess.ProcessID))
                    processServices.Add(item.Status.ServiceStatusProcess.ProcessID, new List<string>());

                processServices[item.Status.ServiceStatusProcess.ProcessID].Add(item.Status.ServiceName);
            }
        }

        public void serviceP_DictionaryModified(ServiceItem oldItem, ServiceItem newItem)
        {
            var oldState = oldItem.Status.ServiceStatusProcess.CurrentState;
            var newState = newItem.Status.ServiceStatusProcess.CurrentState;

            if ((oldState == ServiceState.Paused || oldState == ServiceState.Stopped ||
                oldState == ServiceState.StartPending) &&
                newState == ServiceState.Running)
            {
                this.QueueMessage("Service Started: " + newItem.Status.ServiceName +
                    " (" + newItem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                    ((newItem.Status.DisplayName != "") ?
                    " (" + newItem.Status.DisplayName + ")" :
                    ""), null);

                if (startedSMenuItem.Checked)
                    this.GetFirstIcon().ShowBalloonTip(2000, "Service Started",
                        "The service " + newItem.Status.ServiceName + " (" + newItem.Status.DisplayName + ") has been started.",
                        ToolTipIcon.Info);
            }

            if (oldState == ServiceState.Running &&
                newState == ServiceState.Paused)
                this.QueueMessage("Service Paused: " + newItem.Status.ServiceName +
                    " (" + newItem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                    ((newItem.Status.DisplayName != "") ?
                    " (" + newItem.Status.DisplayName + ")" :
                    ""), null);

            if (oldState == ServiceState.Running &&
                newState == ServiceState.Stopped)
            {
                this.QueueMessage("Service Stopped: " + newItem.Status.ServiceName +
                    " (" + newItem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                    ((newItem.Status.DisplayName != "") ?
                    " (" + newItem.Status.DisplayName + ")" :
                    ""), null);

                if (stoppedSMenuItem.Checked)
                    this.GetFirstIcon().ShowBalloonTip(2000, "Service Stopped",
                        "The service " + newItem.Status.ServiceName + " (" + newItem.Status.DisplayName + ") has been stopped.",
                        ToolTipIcon.Info);
            }
        }

        public void serviceP_DictionaryModified_Process(ServiceItem oldItem, ServiceItem newItem)
        {
            ServiceItem sitem = (ServiceItem)newItem;

            if (sitem.Status.ServiceStatusProcess.ProcessID != 0)
            {
                if (!processServices.ContainsKey(sitem.Status.ServiceStatusProcess.ProcessID))
                    processServices.Add(sitem.Status.ServiceStatusProcess.ProcessID, new List<string>());

                if (!processServices[sitem.Status.ServiceStatusProcess.ProcessID].Contains(
                    sitem.Status.ServiceName))
                    processServices[sitem.Status.ServiceStatusProcess.ProcessID].Add(sitem.Status.ServiceName);

                processServices[sitem.Status.ServiceStatusProcess.ProcessID].Sort();
            }
            else
            {
                int oldId = ((ServiceItem)oldItem).Status.ServiceStatusProcess.ProcessID;

                if (processServices.ContainsKey(oldId))
                {
                    if (processServices[oldId].Contains(
                        sitem.Status.ServiceName))
                        processServices[oldId].Remove(sitem.Status.ServiceName);
                }
            }
        }

        public void serviceP_DictionaryRemoved(ServiceItem item)
        {
            this.QueueMessage("Deleted Service: " + item.Status.ServiceName +
                " (" + item.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                ((item.Status.DisplayName != "") ?
                " (" + item.Status.DisplayName + ")" :
                ""), null);

            if (DSMenuItem.Checked)
                this.GetFirstIcon().ShowBalloonTip(2000, "Service Deleted",
                    "The service " + item.Status.ServiceName + " (" + item.Status.DisplayName + ") has been deleted.",
                    ToolTipIcon.Info);
        }

        public void serviceP_DictionaryRemoved_Process(ServiceItem item)
        {
            if (item.Status.ServiceStatusProcess.ProcessID != 0)
            {
                if (processServices.ContainsKey(item.Status.ServiceStatusProcess.ProcessID))
                {
                    if (processServices[item.Status.ServiceStatusProcess.ProcessID].Contains(
                        item.Status.ServiceName))
                        processServices[item.Status.ServiceStatusProcess.ProcessID].Remove(item.Status.ServiceName);
                }
            }
        }

        #endregion

        #region Service Context Menu

        private void menuService_Popup(object sender, EventArgs e)
        {
            if (listServices.SelectedItems.Count == 0)
            {
                menuService.DisableAll();
                goToProcessServiceMenuItem.Visible = true;
                startServiceMenuItem.Visible = true;
                continueServiceMenuItem.Visible = true;
                pauseServiceMenuItem.Visible = true;
                stopServiceMenuItem.Visible = true;

                selectAllServiceMenuItem.Enabled = true;
            }
            else if (listServices.SelectedItems.Count == 1)
            {
                menuService.EnableAll();

                goToProcessServiceMenuItem.Visible = true;
                startServiceMenuItem.Visible = true;
                continueServiceMenuItem.Visible = true;
                pauseServiceMenuItem.Visible = true;
                stopServiceMenuItem.Visible = true;

                try
                {
                    ServiceItem item = serviceP.Dictionary[listServices.SelectedItems[0].Name];

                    if (item.Status.ServiceStatusProcess.ProcessID != 0)
                    {
                        goToProcessServiceMenuItem.Enabled = true;
                    }
                    else
                    {
                        goToProcessServiceMenuItem.Enabled = false;
                    }

                    if ((item.Status.ServiceStatusProcess.ControlsAccepted & ServiceAccept.PauseContinue)
                        == 0)
                    {
                        continueServiceMenuItem.Visible = false;
                        pauseServiceMenuItem.Visible = false;
                    }
                    else
                    {
                        continueServiceMenuItem.Visible = true;
                        pauseServiceMenuItem.Visible = true;
                    }

                    if (item.Status.ServiceStatusProcess.CurrentState == ServiceState.Paused)
                    {
                        startServiceMenuItem.Enabled = false;
                        pauseServiceMenuItem.Enabled = false;
                    }
                    else if (item.Status.ServiceStatusProcess.CurrentState == ServiceState.Running)
                    {
                        startServiceMenuItem.Enabled = false;
                        continueServiceMenuItem.Enabled = false;
                    }
                    else if (item.Status.ServiceStatusProcess.CurrentState == ServiceState.Stopped)
                    {
                        pauseServiceMenuItem.Enabled = false;
                        stopServiceMenuItem.Enabled = false;
                    }

                    if ((item.Status.ServiceStatusProcess.ControlsAccepted & ServiceAccept.Stop) == 0 &&
                        item.Status.ServiceStatusProcess.CurrentState == ServiceState.Running)
                    {
                        stopServiceMenuItem.Enabled = false;
                    }
                }
                catch
                {
                    menuService.DisableAll();
                    copyServiceMenuItem.Enabled = true;
                    propertiesServiceMenuItem.Enabled = true;
                }
            }
            else
            {
                menuService.DisableAll();

                goToProcessServiceMenuItem.Visible = false;
                startServiceMenuItem.Visible = false;
                continueServiceMenuItem.Visible = false;
                pauseServiceMenuItem.Visible = false;
                stopServiceMenuItem.Visible = false;

                copyServiceMenuItem.Enabled = true;
                propertiesServiceMenuItem.Enabled = true;
                selectAllServiceMenuItem.Enabled = true;
            }

            if (listServices.List.Items.Count == 0)
                selectAllServiceMenuItem.Enabled = false;
        }

        private void goToProcessServiceMenuItem_Click(object sender, EventArgs e)
        {
            this.SelectProcess(
                serviceP.Dictionary[listServices.SelectedItems[0].Name].
                Status.ServiceStatusProcess.ProcessID);
        }

        private void startServiceMenuItem_Click(object sender, EventArgs e)
        {
            ServiceActions.Start(this, listServices.SelectedItems[0].Name, false);
        }

        private void continueServiceMenuItem_Click(object sender, EventArgs e)
        {
            ServiceActions.Continue(this, listServices.SelectedItems[0].Name, false);
        }

        private void pauseServiceMenuItem_Click(object sender, EventArgs e)
        {
            ServiceActions.Pause(this, listServices.SelectedItems[0].Name, false);
        }

        private void stopServiceMenuItem_Click(object sender, EventArgs e)
        {
            ServiceActions.Stop(this, listServices.SelectedItems[0].Name, false);
        }

        private void deleteServiceMenuItem_Click(object sender, EventArgs e)
        {
            if (listServices.SelectedItems.Count != 1)
                return;

            ServiceActions.Delete(this, listServices.SelectedItems[0].Name, true);
        }

        private void propertiesServiceMenuItem_Click(object sender, EventArgs e)
        {
            if (listServices.SelectedItems.Count == 0)
                return;

            List<string> selected = new List<string>();
            ServiceWindow sw;

            foreach (ListViewItem item in listServices.SelectedItems)
                selected.Add(item.Name);

            if (selected.Count == 1)
            {
                sw = new ServiceWindow(selected[0]);
            }
            else
            {
                sw = new ServiceWindow(selected.ToArray());
            }

            sw.TopMost = this.TopMost;
            sw.ShowDialog();
        }

        private void selectAllServiceMenuItem_Click(object sender, EventArgs e)
        {
            Utils.SelectAll(listServices.Items);
        }

        #endregion

        #region Tab Controls

        private void tabControlBig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabNetwork)
            {
                if (processP.RunCount > 0)
                {
                    networkP.Enabled = true;
                    networkP.RunOnceAsync();
                }
            }
            else
            {
                networkP.Enabled = false;
            }
        }

        #endregion

        #region Timers

        private void timerMessages_Tick(object sender, EventArgs e)
        {
            if (statusMessages.Count != 0)
            {
                KeyValuePair<string, Icon> v = statusMessages.Dequeue();
                statusText.Text = v.Key;

                if (v.Value != null)
                    statusIcon.Icon = v.Value;
                else
                    statusIcon.Icon = null;
            }
            else
            {
                statusText.Text = "";
                statusIcon.Icon = null;
            }
        }

        #endregion

        #region ToolStrip Items

        private void findHandlesToolStripButton_Click(object sender, EventArgs e)
        {
            findHandlesMenuItem_Click(sender, e);
        }

        private void refreshToolStripButton_Click(object sender, EventArgs e)
        {
            updateNowMenuItem_Click(sender, e);
        }

        private void sysInfoToolStripButton_Click(object sender, EventArgs e)
        {
            sysInfoMenuItem_Click(sender, e);
        }

        private void optionsToolStripButton_Click(object sender, EventArgs e)
        {
            optionsMenuItem_Click(sender, e);
        }

        #endregion

        #region Trees

        private void treeProcesses_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                terminateMenuItem_Click(null, null);
            }
            else if (e.KeyData == (Keys.Shift | Keys.Delete))
            {
                terminateProcessTreeMenuItem_Click(null, null);
            }
            else if (e.KeyData == Keys.Enter)
            {
                propertiesProcessMenuItem_Click(null, null);
            }
            else if (e.KeyData == (Keys.Control | Keys.M))
            {
                searchProcessMenuItem_Click(null, null);
            }
        }

        private void treeProcesses_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            propertiesProcessMenuItem_Click(null, null);
        }

        private void treeProcesses_SelectionChanged(object sender, EventArgs e)
        {
            processSelectedItems = treeProcesses.SelectedNodes.Count;

            if (processSelectedItems == 1)
            {
                processSelectedPid = treeProcesses.SelectedNodes[0].Pid;
            }
            else
            {
                processSelectedPid = -1;
            }
        }

        #endregion

        #endregion

        #region Form-related Helper functions

        public void ApplyFont(Font f)
        {
            treeProcesses.Tree.Font = f;

            if (f.Height > 16)
                treeProcesses.Tree.RowHeight = f.Height;
            else
                treeProcesses.Tree.RowHeight = 16;

            listServices.List.Font = f;
            listNetwork.List.Font = f;
        }

        public void ClearLog()
        {
            _log.Clear();

            if (this.LogUpdated != null)
                this.LogUpdated(null);
        }

        private void CreateShutdownMenuItems()
        {
            AddMenuItemDelegate addMenuItem = (string text, EventHandler onClick) =>
            {
                shutdownMenuItem.MenuItems.Add(new MenuItem(text, onClick));
                shutdownTrayMenuItem.MenuItems.Add(new MenuItem(text, onClick));
                shutDownToolStripMenuItem.DropDownItems.Add(text, null, onClick);
            };

            addMenuItem("Lock", (sender, e) => { Win32.LockWorkStation(); });
            addMenuItem("Logoff", (sender, e) => { Win32.ExitWindowsEx(ExitWindowsFlags.Logoff, 0); });
            addMenuItem("-", null);
            addMenuItem("Sleep", (sender, e) => { Win32.SetSuspendState(false, false, false); });
            addMenuItem("Hibernate", (sender, e) => { Win32.SetSuspendState(true, false, false); });
            addMenuItem("-", null);
            addMenuItem("Restart", (sender, e) =>
            {
                if (PhUtils.ShowConfirmMessage("restart", "the computer", null, false))
                    Win32.ExitWindowsEx(ExitWindowsFlags.Reboot, 0);
            });
            addMenuItem("Shutdown", (sender, e) =>
            {
                if (PhUtils.ShowConfirmMessage("shutdown", "the computer", null, false))
                    Win32.ExitWindowsEx(ExitWindowsFlags.Shutdown, 0);
            });
            addMenuItem("Poweroff", (sender, e) =>
            {
                if (PhUtils.ShowConfirmMessage("poweroff", "the computer", null, false))
                    Win32.ExitWindowsEx(ExitWindowsFlags.Poweroff, 0);
            });
        }

        public void DeselectAll(ListView list)
        {
            foreach (ListViewItem item in list.SelectedItems)
                item.Selected = false;
        }

        public void DeselectAll(TreeViewAdv tree)
        {
            foreach (TreeNodeAdv node in tree.AllNodes)
                node.IsSelected = false;
        }

        // Technique from http://www.vb-helper.com/howto_2008_uac_shield.html
        private Bitmap GetUacShieldIcon()
        {
            const int width = 50;
            const int height = 50;
            const int margin = 4;
            Bitmap shieldImage;
            Button button = new Button()
            {
                Text = " ",
                Size = new Size(width, height),
                FlatStyle = FlatStyle.System
            };

            button.SetShieldIcon(true);

            Bitmap buttonImage = new Bitmap(width, height);

            button.Refresh();
            button.DrawToBitmap(buttonImage, new Rectangle(0, 0, width, height));

            int minX = width;
            int maxX = 0;
            int minY = width;
            int maxY = 0;

            for (int y = margin; y < height - margin; y++)
            {
                var targetColor = buttonImage.GetPixel(margin, y);

                for (int x = margin; x < width - margin; x++)
                {
                    if (buttonImage.GetPixel(x, y).Equals(targetColor))
                    {
                        buttonImage.SetPixel(x, y, Color.Transparent);
                    }
                    else
                    {
                        if (minY > y) minY = y;
                        if (minX > x) minX = x;
                        if (maxY < y) maxY = y;
                        if (maxX < x) maxX = x;
                    }
                }
            }

            int shieldWidth = maxX - minX + 1;
            int shieldHeight = maxY - minY + 1;

            shieldImage = new Bitmap(shieldWidth, shieldHeight);

            using (Graphics g = Graphics.FromImage(shieldImage))
                g.DrawImage(buttonImage, 0, 0, new Rectangle(minX, minY, shieldWidth, shieldHeight), GraphicsUnit.Pixel);

            buttonImage.Dispose();

            return shieldImage;
        }

        private void LoadWindowSettings()
        {
            this.TopMost = Properties.Settings.Default.AlwaysOnTop;
            this.Size = Properties.Settings.Default.WindowSize;
            this.Location = Utils.FitRectangle(new Rectangle(
                Properties.Settings.Default.WindowLocation, this.Size), this).Location;

            if (Properties.Settings.Default.WindowState != FormWindowState.Minimized)
                this.WindowState = Properties.Settings.Default.WindowState;
            else
                this.WindowState = FormWindowState.Normal;
        }

        private void LoadOtherSettings()
        {
            Utils.UnitSpecifier = Properties.Settings.Default.UnitSpecifier;

            PromptBox.LastValue = Properties.Settings.Default.PromptBoxText;
            toolbarMenuItem.Checked = toolStrip.Visible = Properties.Settings.Default.ToolbarVisible;

            if (Properties.Settings.Default.ToolStripDisplayStyle == 1)
            {
                findHandlesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                sysInfoToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                refreshToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                optionsToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                shutDownToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
            }
            else if (Properties.Settings.Default.ToolStripDisplayStyle == 2)
            {
                refreshToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                optionsToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                shutDownToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                findHandlesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                sysInfoToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;   
            }
            else
            {
                refreshToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
                optionsToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
                shutDownToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
                findHandlesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
                sysInfoToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;   
            }

                ColumnSettings.LoadSettings(Properties.Settings.Default.ProcessTreeColumns, treeProcesses.Tree);
            ColumnSettings.LoadSettings(Properties.Settings.Default.ServiceListViewColumns, listServices.List);
            ColumnSettings.LoadSettings(Properties.Settings.Default.NetworkListViewColumns, listNetwork.List);

            HighlightingContext.Colors[ListViewItemState.New] = Properties.Settings.Default.ColorNew;
            HighlightingContext.Colors[ListViewItemState.Removed] = Properties.Settings.Default.ColorRemoved;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.New] = Properties.Settings.Default.ColorNew;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.Removed] = Properties.Settings.Default.ColorRemoved;

            Program.ImposterNames = new System.Collections.Specialized.StringCollection();

            foreach (string s in Properties.Settings.Default.ImposterNames.Split(','))
                Program.ImposterNames.Add(s.Trim());

            HistoryManager.GlobalMaxCount = Properties.Settings.Default.MaxSamples;
            ProcessHacker.Components.Plotter.GlobalMoveStep = Properties.Settings.Default.PlotterStep;

            // Set up symbols...

            // If this is the first time Process Hacker is being run, try to 
            // set up symbols automatically to make the user happy :).
            if (Properties.Settings.Default.FirstRun)
            {
                string defaultDbghelp = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) +
                    "\\Debugging Tools for Windows (" +
                    (IntPtr.Size == 4 ? "x86" : "x64") + 
                    ")\\dbghelp.dll";

                if (System.IO.File.Exists(defaultDbghelp))
                    Properties.Settings.Default.DbgHelpPath = defaultDbghelp;
            }

            // If we couldn't load dbghelp.dll from the user's location, load the default one 
            // in PATH (usually in system32).
            if (Loader.LoadDll(Properties.Settings.Default.DbgHelpPath) == IntPtr.Zero)
                Loader.LoadDll("dbghelp.dll");

            // Find the location of the dbghelp.dll we loaded and load symsrv.dll.
            try
            {
                ProcessHandle.GetCurrent().EnumModules((module) =>
                    {
                        if (module.FileName.ToLowerInvariant().EndsWith("dbghelp.dll"))
                        {
                            // Load symsrv.dll from the same directory as dbghelp.dll.
                            var fi = new System.IO.FileInfo(module.FileName);

                            Loader.LoadDll(fi.DirectoryName + "\\symsrv.dll");

                            return false;
                        }

                        return true;
                    });
            }
            catch
            { }

            // Set the first run setting here.
            Properties.Settings.Default.FirstRun = false;
        }

        public void QueueMessage(string message)
        {
            this.QueueMessage(message, null);
        }

        public void QueueMessage(string message, Icon icon)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() => this.QueueMessage(message, icon)));
                return;
            }

            var value = new KeyValuePair<DateTime, string>(DateTime.Now, message);

            _log.Add(value);
            statusMessages.Enqueue(new KeyValuePair<string, Icon>(message, icon));

            if (this.LogUpdated != null)
                this.LogUpdated(value);
        }

        private void SaveSettings()
        {
            if (this.WindowState == FormWindowState.Normal && this.Visible)
            {
                Properties.Settings.Default.WindowLocation = this.Location;
                Properties.Settings.Default.WindowSize = this.Size;
            }

            Properties.Settings.Default.AlwaysOnTop = this.TopMost;
            Properties.Settings.Default.WindowState = this.WindowState == FormWindowState.Minimized ?
                FormWindowState.Normal : this.WindowState;
            Properties.Settings.Default.ToolbarVisible = toolStrip.Visible;

            Properties.Settings.Default.PromptBoxText = PromptBox.LastValue;

            Properties.Settings.Default.ProcessTreeColumns = ColumnSettings.SaveSettings(treeProcesses.Tree);
            Properties.Settings.Default.ServiceListViewColumns = ColumnSettings.SaveSettings(listServices.List);
            Properties.Settings.Default.NetworkListViewColumns = ColumnSettings.SaveSettings(listNetwork.List);

            Properties.Settings.Default.NewProcesses = NPMenuItem.Checked;
            Properties.Settings.Default.TerminatedProcesses = TPMenuItem.Checked;
            Properties.Settings.Default.NewServices = NSMenuItem.Checked;
            Properties.Settings.Default.StartedServices = startedSMenuItem.Checked;
            Properties.Settings.Default.StoppedServices = stoppedSMenuItem.Checked;
            Properties.Settings.Default.DeletedServices = DSMenuItem.Checked;

            try
            {
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to save settings", ex);
            }
        }

        public void SelectAll(TreeViewAdv tree)
        {
            foreach (TreeNodeAdv node in tree.AllNodes)
                node.IsSelected = true;
        }

        private void SelectProcess(int pid)
        {
            DeselectAll(treeProcesses.Tree);

            try
            {
                TreeNodeAdv node = treeProcesses.FindTreeNode(pid);

                node.EnsureVisible();
                node.IsSelected = true;
                treeProcesses.Tree.FullUpdate();
                treeProcesses.Tree.Invalidate();

                tabControl.SelectedTab = tabProcesses;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private void UpdateProgram(bool interactive)
        {
            checkForUpdatesMenuItem.Enabled = false;

            Thread t = new Thread(new ThreadStart(() =>
                {
                    Updater.Update(this, interactive);
                    this.Invoke(new MethodInvoker(() => checkForUpdatesMenuItem.Enabled = true));
                }));
            t.IsBackground = true;
            t.Start();
        }

        private void UpdateSessions()
        {
            var currentServer = TerminalServerHandle.GetCurrent();

            usersMenuItem.MenuItems.Clear();

            foreach (var session in currentServer.GetSessions())
            {
                string displayName = session.DomainName + "\\" + session.UserName;

                if (displayName == "\\")
                {
                    // Probably the Services or RDP-Tcp session.
                    session.Dispose();
                    continue;
                }

                MenuItem userMenuItem = new MenuItem();

                userMenuItem.Text = session.SessionId + ": " + displayName;

                MenuItem currentMenuItem;

                currentMenuItem = new MenuItem() { Text = "Disconnect", Tag = session.SessionId };
                currentMenuItem.Click += (sender, e) =>
                    {
                        int sessionId = (int)((MenuItem)sender).Tag;

                        SessionActions.Disconnect(this, sessionId, false);
                    };
                userMenuItem.MenuItems.Add(currentMenuItem);
                currentMenuItem = new MenuItem() { Text = "Logoff", Tag = session.SessionId };
                currentMenuItem.Click += (sender, e) =>
                {
                    int sessionId = (int)((MenuItem)sender).Tag;

                    SessionActions.Logoff(this, sessionId, true);
                };
                userMenuItem.MenuItems.Add(currentMenuItem);
                currentMenuItem = new MenuItem() { Text = "Send Message...", Tag = session.SessionId };
                currentMenuItem.Click += (sender, e) =>
                {
                    int sessionId = (int)((MenuItem)sender).Tag;

                    try
                    {
                        var mbw = new MessageBoxWindow();

                        mbw.MessageBoxTitle = "Message from " + Program.CurrentUsername;
                        mbw.OkButtonClicked += () =>
                            {
                                try
                                {
                                    TerminalServerHandle.GetCurrent().GetSession(sessionId).SendMessage(
                                        mbw.MessageBoxTitle,
                                        mbw.MessageBoxText,
                                        MessageBoxButtons.OK,
                                        mbw.MessageBoxIcon,
                                        0,
                                        0,
                                        mbw.MessageBoxTimeout,
                                        false
                                        );
                                    return true;
                                }
                                catch (Exception ex)
                                {
                                    PhUtils.ShowException("Unable to send the message", ex);
                                    return false;
                                }
                            };
                        mbw.TopMost = this.TopMost;
                        mbw.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        PhUtils.ShowException("Unable to show the message window", ex);
                    }
                };
                userMenuItem.MenuItems.Add(currentMenuItem);
                currentMenuItem = new MenuItem() { Text = "Properties...", Tag = session.SessionId };
                currentMenuItem.Click += (sender, e) =>
                {
                    int sessionId = (int)((MenuItem)sender).Tag;

                    try
                    {
                        var sessionInformationWindow =
                            new SessionInformationWindow(TerminalServerHandle.GetCurrent().GetSession(sessionId));

                        sessionInformationWindow.TopMost = this.TopMost;
                        sessionInformationWindow.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        PhUtils.ShowException("Unable to show session properties", ex);
                    }
                };
                userMenuItem.MenuItems.Add(currentMenuItem);

                usersMenuItem.MenuItems.Add(userMenuItem);
                session.Dispose();
            }
        }

        private void UpdateStatusInfo()
        {
            if (processP.RunCount >= 1)
                statusGeneral.Text = string.Format("{0} processes", processP.Dictionary.Count - 2);
            else
                statusGeneral.Text = "Loading...";

            statusCPU.Text = "CPU: " + (processP.CurrentCpuUsage * 100).ToString("N2") + "%";
            statusMemory.Text = "Phys. Memory: " +
                ((float)(processP.System.NumberOfPhysicalPages - processP.Performance.AvailablePages) * 100 /
                processP.System.NumberOfPhysicalPages).ToString("N2") + "%";
        }

        #endregion

        #region Helper functions

        private void SetProcessPriority(ProcessPriorityClass priority)
        {
            try
            {
                using (var phandle = new ProcessHandle(processSelectedPid, ProcessAccess.SetInformation))
                    phandle.SetPriorityClass(priority);
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to set process priority", ex);
            }
        }

        #endregion

        #region Notification Icons

        public void ExecuteOnIcons(Action<UsageIcon> action)
        {
            foreach (var icon in notifyIcons)
                action(icon);
        }

        public UsageIcon GetFirstIcon()
        {
            foreach (var icon in notifyIcons)
                if (icon.Visible)
                    return icon;

            return dummyIcon;
        }

        public int GetIconsVisibleCount()
        {
            int count = 0;

            foreach (var icon in notifyIcons)
                if (icon.Visible)
                    count++;

            return count;
        }

        public void ApplyIconVisibilities()
        {
            cpuHistoryIcon.Visible = cpuHistoryIcon.Enabled = Properties.Settings.Default.CpuHistoryIconVisible;
            cpuUsageIcon.Visible = cpuUsageIcon.Enabled = Properties.Settings.Default.CpuUsageIconVisible;
            ioHistoryIcon.Visible = ioHistoryIcon.Enabled = Properties.Settings.Default.IoHistoryIconVisible;
            commitHistoryIcon.Visible = commitHistoryIcon.Enabled = Properties.Settings.Default.CommitHistoryIconVisible;
            physMemHistoryIcon.Visible = physMemHistoryIcon.Enabled = Properties.Settings.Default.PhysMemHistoryIconVisible;
        }

        #endregion

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // Magic number - PH uses this to detect previous instances.
                case 0x9991:
                    {
                        this.Visible = true;

                        if (this.WindowState == FormWindowState.Minimized)
                        {
                            this.Location = Properties.Settings.Default.WindowLocation;
                            this.Size = Properties.Settings.Default.WindowSize;
                            this.WindowState = FormWindowState.Normal;
                        }
                        m.Result = new IntPtr(0x1119);

                        return;
                    }
                //break;

                case (int)WindowMessage.SysCommand:
                    {
                        if (m.WParam.ToInt32() == 0xf020) // SC_MINIMIZE
                        {
                            try
                            {
                                if (this.WindowState == FormWindowState.Normal && this.Visible)
                                {
                                    Properties.Settings.Default.WindowLocation = this.Location;
                                    Properties.Settings.Default.WindowSize = this.Size;
                                }

                                if (this.GetIconsVisibleCount() > 0 && Properties.Settings.Default.HideWhenMinimized)
                                {
                                    this.Visible = false;

                                    return;
                                }
                            }
                            catch
                            { }
                        }
                    }
                    break;

                case (int)WindowMessage.Paint:
                    this.Painting();
                    break;

                case (int)WindowMessage.Activate:
                case (int)WindowMessage.KillFocus:
                    {
                        if (treeProcesses != null && treeProcesses.Tree != null)
                            treeProcesses.Tree.Invalidate();
                    }
                    break;

                case (int)WindowMessage.WtsSessionChange:
                    {
                        WtsSessionChangeEvent changeEvent = (WtsSessionChangeEvent)m.WParam.ToInt32();

                        if (
                            changeEvent == WtsSessionChangeEvent.SessionLogon ||
                            changeEvent == WtsSessionChangeEvent.SessionLogoff
                            )
                        {
                            try
                            {
                                this.UpdateSessions();
                            }
                            catch (Exception ex)
                            {
                                Logging.Log(ex);
                            }
                        }
                    }
                    break;

                case (int)WindowMessage.SettingChange:
                    {
                        this.ExecuteOnIcons((icon) => icon.Size = UsageIcon.GetSmallIconSize());
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        public void Exit()
        {
            //processP.Dispose();
            //serviceP.Dispose();
            //networkP.Dispose();

            this.ExecuteOnIcons((icon) => icon.Visible = false);
            SaveSettings();
            this.Visible = false;

            if (KProcessHacker.Instance != null)
                KProcessHacker.Instance.Close();

            try
            {
                Win32.ExitProcess(0);
            }
            catch
            { }
        }

        private void HackerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                this.Exit();
                return;
            }

            if (this.GetIconsVisibleCount() > 0 &&
                Properties.Settings.Default.HideWhenClosed)
            {
                e.Cancel = true;
                showHideMenuItem_Click(sender, null);
                return;
            }

            this.Exit();
        }

        private void CheckedMenuItem_Click(object sender, EventArgs e)
        {
            ((MenuItem)sender).Checked = !((MenuItem)sender).Checked;
        }

        private void UpdateCommon()
        {
            timerMessages.Enabled = true;
            treeProcesses.RefreshItems();
        }

        public void LoadFixMenuItems()
        {
            if (!System.IO.File.Exists(Application.StartupPath + "\\Assistant.exe"))
            {
                runAsServiceMenuItem.Enabled = false;
                runAsProcessMenuItem.Visible = false;
            }

            if (KProcessHacker.Instance == null)
                hiddenProcessesMenuItem.Visible = false;

            if (KProcessHacker.Instance == null || !OSVersion.HasSetAccessToken)
                setTokenProcessMenuItem.Visible = false;

            if (KProcessHacker.Instance == null || !Properties.Settings.Default.EnableExperimentalFeatures)
                protectionProcessMenuItem.Visible = false;

            if (!OSVersion.HasUac)
                virtualizationProcessMenuItem.Visible = false;
        }

        private void LoadFixNProcessHacker()
        {
            bool nphExists, nph32Exists, nph64Exists;
            string startupPath = Application.StartupPath;

            try
            {
                nphExists = System.IO.File.Exists(startupPath + "\\NProcessHacker.dll");
                nph32Exists = System.IO.File.Exists(startupPath + "\\NProcessHacker32.dll");
                nph64Exists = System.IO.File.Exists(startupPath + "\\NProcessHacker64.dll");

                // If we're on 32-bit and NPH32 exists, rename NPH to NPH64 and 
                // NPH32 to NPH.
                if (IntPtr.Size == 4)
                {
                    if (nph32Exists)
                    {
                        if (nphExists)
                            System.IO.File.Move(startupPath + "\\NProcessHacker.dll", startupPath + "\\NProcessHacker64.dll");

                        System.IO.File.Move(startupPath + "\\NProcessHacker32.dll", startupPath + "\\NProcessHacker.dll");
                    }
                }
                // If we're on 64-bit and NPH64 exists, rename NPH to NPH32 and 
                // NPH64 to NPH.
                else if (IntPtr.Size == 8)
                {
                    if (nph64Exists)
                    {
                        if (nphExists)
                            System.IO.File.Move(startupPath + "\\NProcessHacker.dll", startupPath + "\\NProcessHacker32.dll");

                        System.IO.File.Move(startupPath + "\\NProcessHacker64.dll", startupPath + "\\NProcessHacker.dll");
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private void LoadUac()
        {
            if (Program.ElevationType == TokenElevationType.Limited)
            {
                uacShieldIcon = this.GetUacShieldIcon();

                vistaMenu.SetImage(showDetailsForAllProcessesMenuItem, uacShieldIcon);
                //vistaMenu.SetImage(startServiceMenuItem, uacShieldIcon);
                //vistaMenu.SetImage(continueServiceMenuItem, uacShieldIcon);
                //vistaMenu.SetImage(pauseServiceMenuItem, uacShieldIcon);
                //vistaMenu.SetImage(stopServiceMenuItem, uacShieldIcon);
                //vistaMenu.SetImage(deleteServiceMenuItem, uacShieldIcon);
                //runAsServiceMenuItem.Visible = false;
                //runAsProcessMenuItem.Visible = false;
            }
            else
            {
                runAsAdministratorMenuItem.Visible = false;
                showDetailsForAllProcessesMenuItem.Visible = false;
            }
        }

        private void LoadNotificationIcons()
        {
            using (Bitmap b = new Bitmap(16, 16))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.FillRectangle(new SolidBrush(Color.Black), 0, 0, b.Width, b.Height);
                    blackIcon = Icon.FromHandle(b.GetHicon());
                }
            }

            dummyIcon = new UsageIcon();
            notifyIcons.Add(cpuHistoryIcon = new CpuHistoryIcon() { Parent = this });
            notifyIcons.Add(cpuUsageIcon = new CpuUsageIcon() { Parent = this });
            notifyIcons.Add(ioHistoryIcon = new IoHistoryIcon() { Parent = this });
            notifyIcons.Add(commitHistoryIcon = new CommitHistoryIcon() { Parent = this });
            notifyIcons.Add(physMemHistoryIcon = new PhysMemHistoryIcon() { Parent = this });

            foreach (var icon in notifyIcons)
                icon.Icon = (Icon)blackIcon.Clone();

            this.ExecuteOnIcons((icon) => icon.ContextMenu = menuIcon);
            this.ExecuteOnIcons((icon) => icon.MouseDoubleClick += notifyIcon_MouseDoubleClick);
            cpuHistoryMenuItem.Checked = Properties.Settings.Default.CpuHistoryIconVisible;
            cpuUsageMenuItem.Checked = Properties.Settings.Default.CpuUsageIconVisible;
            ioHistoryMenuItem.Checked = Properties.Settings.Default.IoHistoryIconVisible;
            commitHistoryMenuItem.Checked = Properties.Settings.Default.CommitHistoryIconVisible;
            physMemHistoryMenuItem.Checked = Properties.Settings.Default.PhysMemHistoryIconVisible;
            this.ApplyIconVisibilities();

            NPMenuItem.Checked = Properties.Settings.Default.NewProcesses;
            TPMenuItem.Checked = Properties.Settings.Default.TerminatedProcesses;
            NSMenuItem.Checked = Properties.Settings.Default.NewServices;
            startedSMenuItem.Checked = Properties.Settings.Default.StartedServices;
            stoppedSMenuItem.Checked = Properties.Settings.Default.StoppedServices;
            DSMenuItem.Checked = Properties.Settings.Default.DeletedServices;

            NPMenuItem.Click += new EventHandler(CheckedMenuItem_Click);
            TPMenuItem.Click += new EventHandler(CheckedMenuItem_Click);
            NSMenuItem.Click += new EventHandler(CheckedMenuItem_Click);
            startedSMenuItem.Click += new EventHandler(CheckedMenuItem_Click);
            stoppedSMenuItem.Click += new EventHandler(CheckedMenuItem_Click);
            DSMenuItem.Click += new EventHandler(CheckedMenuItem_Click);
        }

        private void LoadControls()
        {
            GenericViewMenu.AddMenuItems(copyProcessMenuItem.MenuItems, treeProcesses.Tree);
            GenericViewMenu.AddMenuItems(copyServiceMenuItem.MenuItems, listServices.List, null);
            GenericViewMenu.AddMenuItems(copyNetworkMenuItem.MenuItems, listNetwork.List, null);

            treeProcesses.ContextMenu = menuProcess;
            listServices.ContextMenu = menuService;
            listNetwork.ContextMenu = menuNetwork;

            processP.Interval = Properties.Settings.Default.RefreshInterval;
            treeProcesses.Provider = processP;
            treeProcesses.Tree.BeginUpdate();
            treeProcesses.Tree.BeginCompleteUpdate();
            this.Cursor = Cursors.WaitCursor;
            processP.Updated += processP_Updated;
            processP.Updated += processP_InfoUpdater;
            if (Program.InspectPid != -1) processP.ProcessQueryReceived += processP_FileProcessingReceived;
            processP.RunOnceAsync();
            processP.Enabled = true;
            updateProcessesMenuItem.Checked = true;

            HighlightingContext.HighlightingDuration = Properties.Settings.Default.HighlightingDuration;
            HighlightingContext.StateHighlighting = false;

            listServices.List.BeginUpdate();
            serviceP.Interval = Properties.Settings.Default.RefreshInterval;
            listServices.Provider = serviceP;
            serviceP.DictionaryAdded += serviceP_DictionaryAdded_Process;
            serviceP.DictionaryModified += serviceP_DictionaryModified_Process;
            serviceP.DictionaryRemoved += serviceP_DictionaryRemoved_Process;
            serviceP.Updated += serviceP_Updated;
            updateServicesMenuItem.Checked = true;

            networkP.Interval = Properties.Settings.Default.RefreshInterval;
            listNetwork.Provider = networkP;

            treeProcesses.Tree.MouseDown += (sender, e) =>
                {
                    if (e.Button == MouseButtons.Right && e.Location.Y < treeProcesses.Tree.ColumnHeaderHeight)
                    {
                        ContextMenu menu = new ContextMenu();

                        menu.MenuItems.Add(new MenuItem("Choose Columns...", (sender_, e_) =>
                            {
                                (new ChooseColumnsWindow(treeProcesses.Tree)
                                {
                                    TopMost = this.TopMost
                                }).ShowDialog();

                                copyProcessMenuItem.MenuItems.DisposeAndClear();
                                GenericViewMenu.AddMenuItems(copyProcessMenuItem.MenuItems, treeProcesses.Tree);
                                treeProcesses.Tree.InvalidateNodeControlCache();
                                treeProcesses.Tree.Invalidate();
                            }));

                        menu.Show(treeProcesses.Tree, e.Location);
                    }
                };
            treeProcesses.Tree.ColumnClicked += (sender, e) => { DeselectAll(treeProcesses.Tree); };
            treeProcesses.Tree.ColumnReordered += (sender, e) =>
            {
                copyProcessMenuItem.MenuItems.DisposeAndClear();
                GenericViewMenu.AddMenuItems(copyProcessMenuItem.MenuItems, treeProcesses.Tree);
            };

            tabControlBig_SelectedIndexChanged(null, null);
        }

        private void LoadAddShortcuts()
        {
            treeProcesses.Tree.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.A)
                    {
                        treeProcesses.TreeNodes.SelectAll();
                        treeProcesses.Tree.Invalidate();
                    }

                    if (e.Control && e.KeyCode == Keys.C) GenericViewMenu.TreeViewAdvCopy(treeProcesses.Tree, -1);
                };
            listServices.List.AddShortcuts();
            listNetwork.List.AddShortcuts();
        }

        private void LoadApplyCommandLineArgs()
        {
            tabControl.SelectedTab = tabControl.TabPages["tab" + Program.SelectTab];
        }

        private void LoadStructs()
        {
            WorkQueue.GlobalQueueWorkItemTag(new Action(() =>
                {
                    try
                    {
                        if (System.IO.File.Exists(Application.StartupPath + "\\structs.txt"))
                        {
                            Structs.StructParser parser = new ProcessHacker.Structs.StructParser(Program.Structs);

                            parser.Parse(Application.StartupPath + "\\structs.txt");
                        }
                    }
                    catch (Exception ex)
                    {
                        QueueMessage("Error loading structure definitions: " + ex.Message);
                    }
                }), "load-structs");
        }

        private void LoadOther()
        {
            statusText.Text = "Waiting...";

            try
            {
                using (var thandle = ProcessHandle.GetCurrent().GetToken(TokenAccess.Query))
                using (var sid = thandle.GetUser())
                    this.Text += " [" + sid.GetFullName(true) + (KProcessHacker.Instance != null ? "+" : "") + "]";
            }
            catch
            { }

            // If it's Vista and we're elevated, we should allow the magic window message to allow 
            // Allow only one instance to work.
            if (OSVersion.HasUac &&
                Program.ElevationType == TokenElevationType.Full)
            {
                this.Text += " (Administrator)";
                Win32.ChangeWindowMessageFilter((WindowMessage)0x9991, UipiFilterFlag.Add);
            }
        }

        public HackerWindow()
        {
            Program.HackerWindow = this;
            processP = Program.ProcessProvider;
            serviceP = Program.ServiceProvider;
            networkP = Program.NetworkProvider;

            InitializeComponent();

            CreateHackerMainMenu();

            this.AddEscapeToClose();

            // Force the handle to be created
            { var handle = this.Handle; }
            Program.HackerWindowHandle = this.Handle;

            Logging.Logged += this.QueueMessage;
            Settings.Refresh();
            this.LoadWindowSettings();
            this.LoadOtherSettings();
            this.LoadControls();
            this.LoadNotificationIcons();

            if ((!Properties.Settings.Default.StartHidden && !Program.StartHidden) ||
                Program.StartVisible)
            {
                this.Visible = true;
            }

            if (tabControl.SelectedTab == tabProcesses)
                treeProcesses.Tree.Select();

            this.LoadOther();
            this.LoadStructs();

            vistaMenu.DelaySetImageCalls = false;
            vistaMenu.PerformPendingSetImageCalls();
            serviceP.RunOnceAsync();
            serviceP.Enabled = true;

            _dontCalculate = false;
        }

        private void HackerWindow_Load(object sender, EventArgs e)
        {
            Program.UpdateWindowMenu(windowMenuItem, this);
            this.ApplyFont(Properties.Settings.Default.Font);
            this.BeginInvoke(new MethodInvoker(this.LoadApplyCommandLineArgs));
        }

        private void HackerWindow_SizeChanged(object sender, EventArgs e)
        {
            tabControl.Invalidate(false);
        }

        private void HackerWindow_VisibleChanged(object sender, EventArgs e)
        {
            treeProcesses.Draw = this.Visible;
        }

        // ==== Performance hacks section ====
        private bool _dontCalculate = true;
        private int _layoutCount = 0;

        protected override void OnLayout(LayoutEventArgs levent)
        {
            _layoutCount++;

            if (_layoutCount < 3)
                return;

            base.OnLayout(levent);
        }

        protected override void OnResize(EventArgs e)
        {
            if (_dontCalculate)
                return;

            //
            // Size grip bug fix as per
            // http://jelle.druyts.net/2003/10/20/StatusBarResizeBug.aspx
            //
            if (statusBar != null)
            {
                statusBar.SizingGrip = (WindowState == FormWindowState.Normal);
            }

            base.OnResize(e);
        }

        private bool isFirstPaint = true;

        private void Painting()
        {
            if (isFirstPaint)
            {
                isFirstPaint = false;
                this.CreateShutdownMenuItems();
                this.LoadFixMenuItems();
                this.LoadUac();
                this.LoadAddShortcuts();
                this.LoadFixNProcessHacker();

                toolStrip.Items.Add(new ToolStripSeparator());
                var targetButton = new TargetWindowButton();
                targetButton.TargetWindowFound += (pid, tid) => this.SelectProcess(pid);
                toolStrip.Items.Add(targetButton);

                var targetThreadButton = new TargetWindowButton();
                targetThreadButton.TargetWindowFound += (pid, tid) =>
                    {
                        Program.GetProcessWindow(processP.Dictionary[pid], (f) =>
                            {
                                Program.FocusWindow(f);
                                f.SelectThread(tid);
                            });
                    };
                targetThreadButton.Image = Properties.Resources.application_go;
                targetThreadButton.Text = "Find window and select thread";
                targetThreadButton.ToolTipText = "Find window and select thread";
                toolStrip.Items.Add(targetThreadButton);

                try { TerminalServerHandle.RegisterNotificationsCurrent(this, true); }
                catch (Exception ex) { Logging.Log(ex); }
                try { this.UpdateSessions(); }
                catch (Exception ex) { Logging.Log(ex); }

                try { Win32.SetProcessShutdownParameters(0x100, 0); }
                catch { }

                if (Properties.Settings.Default.AppUpdateAutomatic)
                    this.UpdateProgram(false);
            }
        }
    }
}
