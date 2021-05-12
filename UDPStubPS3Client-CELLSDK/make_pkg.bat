@ECHO OFF
title make_package_custom
if not exist custom md custom

:: ----------------------------------------------
:: Simple script to build a PKG (by CaptainCPS-X)
:: ----------------------------------------------

:: Change these for your application / manual...
set CID=EP0001-NETSTUB_00-0000000000000000
set PKG_DIR=./bin/
set PKG_NAME=NetStub.pkg

pkg_custom.exe --contentid %CID% %PKG_DIR% %PKG_NAME%
