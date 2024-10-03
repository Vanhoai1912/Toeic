$(document).ready(function () {
    $('.nav-linkH').click(function (e) {
        e.preventDefault();
        const linkId = $(this).attr('id');
        let url = '';

        // Xác định URL dựa trên id của liên kết
        if (linkId === 'homeLink') {
            url = '/Customer/Home/Index';
        } else if (linkId === 'vocabularyLink') {
            url = '/Customer/Voca/Index';
        } else if (linkId === 'grammarLink') {
            url = '/Customer/Grammar/Index';
        } else if (linkId === 'adminLink') {
            window.location.href = '/Admin/HomeAdmin/Index'; // Điều hướng trực tiếp đến Admin
            return;
        } else if (linkId === 'login') {
            url = '/Identity/Account/Login';
        } else if (linkId === 'register') {
            url = '/Identity/Account/Register';
        } else if (linkId === 'manage') {
            url = '/Identity/Account/Logout';
        }


        if (url !== '') {
            $.ajax({
                url: url,
                type: 'GET',
                success: function (data) {
                    var parsed = $(data).find('#content').html();
                    $('#content').html(parsed);
                    window.history.pushState({ path: url }, '', url);
                    $('.nav-linkH').removeClass('active');
                    $(this).addClass('active');
                }.bind(this),
                error: function () {
                    alert('Error while loading data.');
                }
            });
        }
    });

    window.onpopstate = function (event) {
        if (event.state) {
            $.ajax({
                url: event.state.path,
                type: 'GET',
                success: function (data) {
                    var parsed = $(data).find('#content').html();
                    $('#content').html(parsed);
                    $('.nav-linkH').removeClass('active');
                    $('.nav-linkH[id="' + getActiveLinkId(event.state.path) + '"]').addClass('active');
                },
                error: function () {
                    alert('Error while loading data.');
                }
            });
        }
    };

    $('#backLink').click(function (e) {
        e.preventDefault();
        history.back();
    });

    function getActiveLinkId(path) {
        switch (path) {
            case '/Customer/Home/Index':
                return 'homeLink';
            case '/Customer/Voca/Index':
                return 'vocabularyLink';
            case '/Customer/Grammar/Index':
                return 'grammarLink';
            case '/Identity/Account/Login':
                return 'login';
            case '/Identity/Account/Register':
                return 'register';
            default:
                return '';
        }
    }
});


