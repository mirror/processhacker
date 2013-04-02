@echo off
rem install node.js and then run:
rem npm install -g clean-css

pushd %~dp0

echo minifying and combining css files...
type css\stylesheet.css css\normalize.css | cleancss --s0 --remove-empty -o css\pack.css

popd
