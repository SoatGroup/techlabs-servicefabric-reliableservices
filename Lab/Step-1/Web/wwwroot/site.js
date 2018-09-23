function getItems(id) {
    console.log('Get Item :' + id)
    $.ajax({
        type: 'GET',
        url: 'api/rss/' + id,
        success: function (data) {
            $('#posts').empty();

            $.each(data, function (key, item) {

                $('<tr>' +
                    '<td>' + item.title + '</td>' +
                    '<td><a href="' + item.link + '" target="_blank">' + item.link + '</a></td>' + 
                  '</tr>').appendTo($('#posts'));
            });
        }
    });
}

function getBlogs() {
    $.ajax({
        type:'GET',
        url: 'api/rss/blogs',
        success: function (data) {
            $('#blog').empty();

            $.each(data, function (key, item) {

                $('<tr>' +
                    '<td>' + item.id + '</td>' +
                    '<td>' + item.blogName + '</td>' + 
                    '<td>' + item.url + '</td>' + 
                    '<td><button onclick="getItems(\'' + item.id + '\')">Get Items</button></td>' +
                  '</tr>').appendTo($('#blog'));
            });
        }
    })
}

function sample() {
     $.ajax({
        type: 'POST',
        accepts: 'application/json',
        url: 'api/rss/sample',
        contentType: 'application/json',
        data: '',
        error: function (jqXHR, textStatus, errorThrown) {
            alert('here');
        },
        success: function (result) {
            getBlogs();
        }
    });
}