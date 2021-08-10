layui.define(function (exports) {

    $ = layui.$;
    var YDHConfig = layui.data('YDH');

    if (!YDHConfig.Secret) {
        window.location = '/views/login.html';
    }

    $.ajaxSetup({
        beforeSend: function (req) {
            var YDHConfig = layui.data('YDH');

            if (!YDHConfig.Secret) {
                window.location = '/views/login.html';
            }
            req.setRequestHeader("YDHAuthToken", YDHConfig.Secret);
        },
        complete: function (err) {
            if (err.status === 401)
            {
                layui.data('YDH', {
                    key: 'Secret'
                    , remove: true
                });
                layui.data('YDH', {
                    key: 'UserName'
                    , remove: true 
                });
                layui.data('YDH', {
                    key: 'UserType'
                    , remove: true
                });
                window.location = '/views/login.html';
            } 
        }
    });


    var app = {};
    exports("app", app);
});

