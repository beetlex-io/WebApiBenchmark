/************************************************************************************
FastHttpApi javascript api Generator Copyright © henryfan 2018 email:henryfan@msn.com
https://github.com/IKende/FastHttpApi
**************************************************************************************/




var WebApiListServerUrl = '/ListServer';
/**
* 'WebApiListServer(params).execute(function(result){});'
**/
function WebApiListServer(useHttp) {
    return api(WebApiListServerUrl, {}, useHttp);
}
var WebApiAddServerUrl = '/AddServer';
/**
* 'WebApiAddServer(params).execute(function(result){});'
**/
function WebApiAddServer(server, useHttp) {
    return api(WebApiAddServerUrl, { server: server }, useHttp);
}
var WebApiDelServerUrl = '/DelServer';
/**
* 'WebApiDelServer(params).execute(function(result){});'
**/
function WebApiDelServer(server, useHttp) {
    return api(WebApiDelServerUrl, { server: server }, useHttp);
}
var WebApiCreateUrl = '/Create';
/**
* 'WebApiCreate(params).execute(function(result){});'
**/
function WebApiCreate(useHttp) {
    return api(WebApiCreateUrl, {}, useHttp);
}
var WebApiGetUrl = '/Get';
/**
* 'WebApiGet(params).execute(function(result){});'
**/
function WebApiGet(id, useHttp) {
    return api(WebApiGetUrl, { id: id }, useHttp);
}
var WebApiDeleteWithCategoryUrl = '/DeleteWithCategory';
/**
* 'WebApiDeleteWithCategory(params).execute(function(result){});'
**/
function WebApiDeleteWithCategory(category, useHttp) {
    return api(WebApiDeleteWithCategoryUrl, { category: category }, useHttp);
}
var WebApiDeleteUrl = '/Delete';
/**
* 'WebApiDelete(params).execute(function(result){});'
**/
function WebApiDelete(id, useHttp) {
    return api(WebApiDeleteUrl, { id: id }, useHttp);
}
var WebApiSaveUrl = '/Save';
/**
* 'WebApiSave(params).execute(function(result){});'
**/
function WebApiSave(requestSetting, useHttp) {
    return api(WebApiSaveUrl, { requestSetting: requestSetting }, useHttp);
}
var WebApiListCategoryUrl = '/ListCategory';
/**
* 'WebApiListCategory(params).execute(function(result){});'
**/
function WebApiListCategory(useHttp) {
    return api(WebApiListCategoryUrl, {}, useHttp);
}
var WebApiListUrl = '/List';
/**
* 'WebApiList(params).execute(function(result){});'
**/
function WebApiList(category, useHttp) {
    return api(WebApiListUrl, { category: category }, useHttp);
}
var WebApiPerformanceTestStatusUrl = '/PerformanceTestStatus';
/**
* 'WebApiPerformanceTestStatus(params).execute(function(result){});'
**/
function WebApiPerformanceTestStatus(useHttp) {
    return api(WebApiPerformanceTestStatusUrl, {}, useHttp);
}
var WebApiUnitTestUrl = '/UnitTest';
/**
* 'WebApiUnitTest(params).execute(function(result){});'
**/
function WebApiUnitTest(c, host, useHttp) {
    return api(WebApiUnitTestUrl, { c: c, host: host }, useHttp);
}
var WebApiPerformanceTestStopUrl = '/PerformanceTestStop';
/**
* 'WebApiPerformanceTestStop(params).execute(function(result){});'
**/
function WebApiPerformanceTestStop(useHttp) {
    return api(WebApiPerformanceTestStopUrl, {}, useHttp);
}
var WebApiPerformanceTestUrl = '/PerformanceTest';
/**
* 'WebApiPerformanceTest(params).execute(function(result){});'
**/
function WebApiPerformanceTest(setting, cases, useHttp) {
    return api(WebApiPerformanceTestUrl, { setting: setting, cases: cases }, useHttp);
}
var WebApiTestCaseUrl = '/TestCase';
/**
* 'WebApiTestCase(params).execute(function(result){});'
**/
function WebApiTestCase(requestSetting, host, useHttp) {
    return api(WebApiTestCaseUrl, { requestSetting: requestSetting, host: host }, useHttp);
}
var WebApiDownloadUrl = '/Download';
/**
* 'WebApiDownload(params).execute(function(result){});'
**/
function WebApiDownload(useHttp) {
    return api(WebApiDownloadUrl, {}, useHttp);
}
var WebApiGetFileIDUrl = '/GetFileID';
/**
* 'WebApiGetFileID(params).execute(function(result){});'
**/
function WebApiGetFileID(useHttp) {
    return api(WebApiGetFileIDUrl, {}, useHttp);
}
var WebApiUploadUrl = '/Upload';
/**
* 'WebApiUpload(params).execute(function(result){});'
**/
function WebApiUpload(name, completed, data, useHttp) {
    return api(WebApiUploadUrl, { name: name, completed: completed, data: data }, useHttp);
}
