function CallApi(api) {
    var name = $("#documentName")[0].value;
    switch (api) {
        case "Free":
            $.ajax({
                type: "GET",
                url: "/Company/GetFreeDocument/"+name,
                success: function (data) {
                    $("#result").text(data);
                },
                error: function (data) {
                    if (data.status === 403) {
                        $("#result").text("User not Authorized");
                    } else {
                        $("#result").text(data.responseText);
                    }
                }
            });
            break;
        case "Personal":
            $.ajax({
                type: "GET",
                url: "/Company/GetPersonalDocument/"+name,
                success: function (data) {
                    $("#result").text(data);
                },
                error: function (data) {
                    if (data.status === 403) {
                        $("#result").text("User not Authorized");
                    } else {
                        $("#result").text(data.responseText);
                    }
                }
            });
            break;
    }
}