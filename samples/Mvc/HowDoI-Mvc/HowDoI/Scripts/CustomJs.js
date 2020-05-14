var coderAccess = false;
var controllerAccess = false;
var oldLink = null;
var invokeActionSubHost = null;
var invokeActionProtocol = null;

function invokeAction(protocol, subhost, controller, action, params, callback) {
    invokeActionSubHost = subhost;
    invokeActionProtocol = protocol;

    var url = protocol + "://" + window.location.host + subhost + '/' + controller + '/' + action + "?" + params;
    var wRequest = new Sys.Net.WebRequest();
    wRequest.set_url(url);
    wRequest.set_httpVerb('GET');
    wRequest.add_completed(callback);
    wRequest.invoke();
}

function sourceodeCallback(result) {
    if (result.get_statusCode() !== '404') {
        var response = result.get_responseData();

        var path = invokeActionProtocol + "://" + window.location.host + invokeActionSubHost + '/Resources/SourceCode/' + response + '.htm';
        var newWindow = window.open(path, '');
        newWindow.focus();
    }
}