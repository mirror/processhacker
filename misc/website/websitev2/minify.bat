@echo off
rem install node.js and then run:
rem npm install -g clean-css
rem npm install -g uglify-js

pushd %~dp0

echo minifying and combining css files...
type css\stylesheet.css css\normalize.css | cleancss --s0 --remove-empty -o css\pack.css
cmd /c uglifyjs js\moment.js --compress --mangle -o js\moment.min.js

popd
