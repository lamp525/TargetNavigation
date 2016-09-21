//原图/缩略图 的比例 >=1
var UserHeadUtil = {
    ratio: 1,
    ratioW: 1,
    ratioH: 1,
    view_H: 480,
    view_W: 480,
    initialize: function (path, positon) {
        $("#user_head_origin").attr("src", path);
        $("#user_head_upload_box").hide();
        $("#user_head_show_box").show();

        //$("#user_head_25").attr("src", path);
        $("#user_head_50").attr("src", path);
        $("#user_head_75").attr("src", path);
        $("#user_head_180").attr("src", path);
        var img = new Image();
        img.src = path;
        if (img.width == 0) {
            var obj = this;
            img.onload = function () {
                obj.imgOperate(img, positon);
            };
        } else {
            this.imgOperate(img, positon);
        }
    },
    imgOperate: function (img, positon) {
        if (img) {
            this.resize('user_head_origin', img.width, img.height, 480, 480);
            if (positon == null) {
                var x = 0, y = 0, size = 0, showsize = 0;
                if (this.view_W > this.view_H) {
                    x = (this.view_W - this.view_H) / 2;
                    size = this.view_H;
                    showsize = this.view_H;
                } else if (this.view_W < this.view_H) {
                    y = (this.view_H - this.view_W) / 2;
                    size = this.view_W;
                    showsize = this.view_W;
                } else {
                    size = this.view_W;
                    showsize = this.view_W;
                }

            }
            else {
                var zb = positon.split(',');
                var x = zb[0], y = zb[1], size = zb[2], showsize = zb[2];
                if (this.view_W > this.view_H) {
                    size = this.view_H;
                } else if (this.view_W < this.view_H) {
                    size = this.view_W;
                } else {
                    size = this.view_W;
                }
            }
            var obj = this;
            mytestEl = $('img#user_head_origin').imgAreaSelect({
                aspectRatio: "1:1",
                handles: "corners",
                persistent: true,
                show: true,
                imageWidth: obj.view_W,
                imageHeight: obj.view_H,
                onSelectChange: function (img, selection) {
                    obj.preview('user_head_50', obj.view_W, obj.view_H, selection.x1, selection.y1, selection.width, selection.height, 32, 32);
                    obj.preview('user_head_75', obj.view_W, obj.view_H, selection.x1, selection.y1, selection.width, selection.height, 64, 64);
                    obj.preview('user_head_180', obj.view_W, obj.view_H, selection.x1, selection.y1, selection.width, selection.height, 140, 140);
                    obj.setCutParams(selection.x1, selection.y1, selection.width, selection.height);
                }
                ,
                x1: x,
                y1: y,
                x2: parseInt(x) + parseInt(showsize),
                y2: parseInt(y) + parseInt(showsize),

            });
            this.preview('user_head_50', this.view_W, this.view_H, x, y, showsize, showsize, 32, 32);
            this.preview('user_head_75', this.view_W, this.view_H, x, y, showsize, showsize, 64, 64);
            this.preview('user_head_180', this.view_W, this.view_H, x, y, showsize, showsize, 140, 140);
            this.setCutParams(x, y, size, size);
        }
    },
    resize: function (id, width, height, limit_W, limit_H) {
        if (width > 0 && height > 0) {
            if (width / height >= limit_W / limit_H) {
                if (width > limit_W) {
                    this.view_W = limit_W;
                    this.view_H = (limit_W / width) * height;
                    this.ratioW = (width / this.view_W);
                    this.ratioH = (height / this.view_H);
                }
            } else {
                if (height > limit_H) {
                    this.view_H = limit_H;
                    this.view_W = (limit_H / height) * width;
                    this.ratioW = (width / this.view_W);
                    this.ratioH = (height / this.view_H);
                }
            }
            $('#' + id).attr({
                "width": this.view_W,
                "height": this.view_H
            });
            this.ratio = width / this.view_W;
        }
        var imgWidth = parseInt($('#' + id).width());
        var imgHeight = parseInt($('#' + id).height());
        $('#' + id).css({ 'margin-left': (854 - imgWidth) / 2, 'margin-top': (480 - imgHeight) / 2 });
    },

    preview: function (id, width, height, x, y, cut_W, cut_H, show_W, show_H) {
        var scaleX = show_W / (cut_W  || 1);
        var scaleY = show_H / (cut_H || 1);
        $('#' + id).css({
            width: Math.round(scaleX * width) + 'px',
            height: Math.round(scaleY * height ) + 'px',
            marginLeft: '-' + Math.round(scaleX * x ) + 'px',
            marginTop: '-' + Math.round(scaleY * y) + 'px'
        });
    },
    setCutParams: function (x, y, width, height) {
        $('#head_selectX').val(x);
        $('#head_selectY').val(y);
        $('#head_x').val(Math.round(x * this.ratio));
        $('#head_y').val(Math.round(y * this.ratio));
        $('#head_width').val(Math.round(width * this.ratio));
        $('#head_height').val(Math.round(height * this.ratio));
        $('#head_ratiow').val(Math.round(width));
        $('#head_ratioh').val(Math.round(height));
    }
};

function cancelHead() {
    //	window.location.reload();
    $('img#user_head_origin').imgAreaSelect({ remove: true });
    //$("#user_head_show_box").hide();
    //$("#user_head_upload_box").show();
    var path = $("img#origin_user_head_75").attr("src");
    var index = path.lastIndexOf("/");
    if (index != -1) {
        var name = path.substring(index + 1);
        $("#user_head_50").attr("src", "/Images/common/portrait.png").css({
            width: 32 + 'px',
            height: 32 + 'px',
            marginLeft: 0,
            marginTop: 0
        });
        $("#user_head_75").attr("src", "/Images/common/portrait.png").css({
            width: 64 + 'px',
            height: 64 + 'px',
            marginLeft: 0,
            marginTop: 0
        });
        $("#user_head_180").attr("src", "/Images/common/portrait.png").css({
            width: 140 + 'px',
            height: 140 + 'px',
            marginLeft: 0,
            marginTop: 0
        });
    }
}