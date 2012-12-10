@echo off
rem install node.js and then run:
rem npm install -g clean-css
rem npm install -g uglify-js

pushd %~dp0

echo minifying and combining css and js files...
type css\jquery.fancybox.css css\stylesheet.css css\normalize.css | cleancss --s0 -o css\pack.css
cmd /c uglifyjs js\jquery.fancybox.js js\jquery.mousewheel.js --compress --mangle -o js\pack.js

popd
