function AutoRemindersWidgetRefresher() {
    setInterval(RefreshRemindersWidget(), 60000);
}

function RefreshRemindersWidget() {
    $('.rems-fld').load('/Reminders/RemindersWidget/');
}

function ReminderForModule(moduleId, moduleName, titleAppend) {
    //yahan pe new reminder wala popup open krna hai..
    EmptyModel();
    $('#ModalLabel').text("Create New Reminder");
    $('#modalbody').load('/Reminders/NewReminder?modId=' + moduleId + '&mode=' + moduleName + '&title=' + titleAppend);
}