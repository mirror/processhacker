@echo off
rem install node.js and then run:
rem npm install -g clean-css
rem npm install -g uglify-js

pushd %~dp0

echo minifying and combining css files...
cmd /c cleancss --s0 --compatibility ie8  css\normalize.css css\stylesheet.css -o css\pack.css
cmd /c uglifyjs js\moment.js js\feed.js --compress --mangle -o js\pack.js

popd
