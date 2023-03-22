$(document).ready(function () {
    SiteIsReady();
});

function SiteIsReady() {
    $(document).on("click", '[data-click-result="dialog"]', dlg.assumeDialog);
    $("[data-ajax-auto-load]").each(ajax.autoLoad);
}


class dlg {
    static assumeDialog(e) {
        e.preventDefault();
        let $this = $(this);
        let url = $this.data("url");
        return ajax.start(url, {}, "GET")
            .then(resp => resp.text())
            .then(dlg.show)
            .catch(resp => ajax.errAsYouCanResp(resp));

        return false;
    }

    static close() {
            $('.modal').modal('hide');
    }

    static show(html) {
        dlg.close();
        var $dialog = $(html);
        $("body").prepend($dialog);
        $dialog.modal({ show: true, keyboard: true, backdrop: 'static' });
    }
}


class ajax {
    static start(url, urlParams, method, headers) {
        if (method.toUpperCase() === "GET" && urlParams !== null) {
            url += (url.includes("?") ? "&" : "?");
            url = url + new URLSearchParams(urlParams);
            urlParams = null;
        }

        return fetch(url, {
            method: method,
            body: urlParams === null ? null : urlParams,
            headers: headers,
            cache: "no-store"
        });
    }


    static autoLoad() {
        let $this = $(this);
        let url = $this.data("ajax-auto-load");
        let targetId = $this.data("target");
        ajax.start(url, {}, "GET")
            .then(resp => resp.text())
            .then(content => $(targetId).html(content));
    }

}