'use strict';

const path = require('path');
const edge = require('edge-js');

const dllPath = path.join(__dirname, './lib/dll/release/FTPLibrary.dll');

const dllLib = {};

/**
 * Method: Create local ftp
 * @param {string} role ftp user rold
 * @param {string} siteName ftp site name
 * @param {string} serverIp  ftp server ip
 * @param {string} physicalPath file path of ftp server
 */
dllLib.ftp = edge.func({
    assemblyFile: dllPath,
    typeName: 'Ftp.Ftp',
    methodName: 'AddFtp'
})

module.exports = dllLib