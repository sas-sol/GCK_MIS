function getnotifications() {
    $.ajax({
        url: '/Notify/TotalNotifications/',
        type: 'post',
        success: function (data) {
            
            if (data.nots > 0) {
                $('.msgcount1').show();
                $('.noti-noti-counter').empty();
                if (data.nots <= 99) {
                    $('.noti-counter').html(data.nots);
                }
                else {
                    $('.noti-counter').html('99+');
                }
                var ttl = data.nots;
                if (ttl <= 99) {
                    document.title = "(" + ttl + ") MIS";
                }
                else {
                    document.title = "(99+) MIS";
                }
            }
            else {
                $('.msgcount1').hide();
            }

            if (data.msg > 0) {
                $('.msgscount').show();
                if (data.msg <= 99) {
                    $('.msg_count').html(data.msg);
                }
                else {
                    $('.msg_count').html('99+');
                }
                var ttl = data.nots + data.msg;
                if (ttl <= 99) {
                    document.title = "(" + ttl + ") MIS";
                }
                else {
                    document.title = "(99+) MIS";
                }
            }
            else {
                $('.msgscount').hide();
            }
        },
        error: function () {
        }
    });
}
//
function NotiAppend(list) {
    $(".noti-list").prepend(list);
}
//
function Shownotification(e) {
    $.notiny({
        text: e,
        width: 'auto'
    });
}