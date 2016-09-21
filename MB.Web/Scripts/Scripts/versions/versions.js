/**
 * Created by ZETA on 2015/12/30.
 */
define(['artTemplate'], function (template) {
    function renderVersionsInfo(data) {
        var $versions = $("#versions_info").empty();

        var html = template('template_version', data);
        $versions.html(html);
    }

    return {
        render: function () {
            $.ajax({
                url: "/Version/GetVersionList",
                type: "post",
                dataType: "json",
                data: {},
                success: function (result) {
                    renderVersionsInfo(result.data);
                }
            })
        }
    }
});