(function() {
  var $, AirMessage, spinner_template,
    __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  $ = jQuery;

  spinner_template = '<div class="messenger-spinner">\n    <span class="messenger-spinner-side messenger-spinner-side-left">\n        <span class="messenger-spinner-fill"></span>\n    </span>\n    <span class="messenger-spinner-side messenger-spinner-side-right">\n        <span class="messenger-spinner-fill"></span>\n    </span>\n</div>';

  AirMessage = (function(_super) {

    __extends(AirMessage, _super);

    function AirMessage() {
      return AirMessage.__super__.constructor.apply(this, arguments);
    }

    AirMessage.prototype.template = function(opts) {
      var $message;
      $message = AirMessage.__super__.template.apply(this, arguments);
      $message.append($(spinner_template));
      return $message;
    };

    return AirMessage;

  })(window.Messenger.Message);

  window.Messenger.themes.air = {
    Message: AirMessage
  };

}).call(this);
