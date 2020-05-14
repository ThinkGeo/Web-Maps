/*
 * A colorselector for Twitter Bootstrap which lets you select a color from a predefined set of colors only.
 * https://github.com/flaute/bootstrap-colorselector
 *
 * Copyright (C) 2014 Flaute
 *
 * Licensed under the MIT license
 */

(function($) {
  "use strict";

  var ColorSelector = function(select, options) {
    this.options = options;
    this.$select = $(select);
    this._init();
  };

  ColorSelector.prototype = {

    constructor : ColorSelector,

    _init : function() {

      var callback = this.options.callback;

      var selectValue = this.$select.val();
      var selectColor = this.$select.find("option:selected").data("color");

      var $markupUl = $("<ul>").addClass("dropdown-menu").addClass("dropdown-caret");
      var $markupDiv = $("<div>").addClass("dropdown").addClass("dropdown-colorselector").addClass("dropdown-colorselectorContainer");
      var $markupSpan = $("<span>").addClass("btn-colorselector").css("background-color", selectColor);
      var $markupA = $("<a>").attr("data-toggle", "dropdown").addClass("dropdown-toggle").attr("href", "#").append($markupSpan);

      // create an li-tag for every option of the select
      $("option", this.$select).each(function() {

        var option = $(this);
        var value = option.attr("value");
        var color = option.data("color");
        var title = option.text();

        // create a-tag
        var $markupA = $("<a>").addClass("color-btn");
        if (option.prop("selected") === true || selectValue === color) {
          $markupA.addClass("selected");
        }
        $markupA.css("background-color", color);
        $markupA.attr("href", "#").attr("data-color", color).attr("data-value", value).attr("title", title);

        // create li-tag
        $markupUl.append($("<li>").append($markupA));
      });

      // append the colorselector
      $markupDiv.append($markupA);
      // append the colorselector-dropdown
      $markupDiv.append($markupUl);

      // hide the select
      this.$select.hide();
      
      // insert the colorselector
      this.$selector = $($markupDiv).insertAfter(this.$select);

      // register change handler
      this.$select.on("change", function() {

        var value = $(this).val();
        var name = $(this).find("option[value='" + value + "']").text();

        // remove old and set new selected color
        $(this).next().find("ul").find("li").find(".selected").removeClass("selected");
        $(this).next().find("ul").find("li").find("a[data-color='" + value + "']").addClass("selected");

        $(this).next().find(".btn-colorselector").css("background-color", value);
        var valueSpan = $(this).next().find(".btn-colorselector").parent();
        if(valueSpan.find("#selectedColorValue").length > 0){
            valueSpan.find("#selectedColorValue").text(name);
        }else{
            valueSpan.append("<span id='selectedColorValue' class='.selected-value' style='lineHeight:20px;verte'>" + name +"</span>");
        }
        
        callback(value, name);
      });

      // register click handler
      $markupUl.on('click.colorselector', $.proxy(this._clickColor, this));
    },

    _clickColor : function(e) {

      var a = $(e.target);

      if (!a.is(".color-btn")) {
        return false;
      }

      this.$select.val(a.data("value")).change();

      e.preventDefault();
      return true;
    },

    setColor : function(color) {
      // find value for color
      var value = $(this.$selector).find("li").find("a[data-color='" + color + "']").data("value");
      this.setValue(value);
      
      var valueSpan = $(this).next().find(".btn-colorselector").parent();
      var name = $(this.$selector).find("li").find("a[data-color='" + color + "']").attr("title")
      if(valueSpan.find("#selectedColorValue").length > 0){
          valueSpan.find("#selectedColorValue").text(name);
      }else{
          valueSpan.append("<span id='selectedColorValue' class='.selected-value'>" + name +"</span>");
      }
    },

    setValue : function(value) {
      this.$select.val(value).change();
    },

  };

  $.fn.colorselector = function(option) {
    var args = Array.apply(null, arguments);
    for (var i = 0; i < args.length; i++) {
        if(args[i]["callback"]){
            $.fn.colorselector.defaults.callback = args[i]["callback"];
        }
    };
    args.shift();

    return this.each(function() {

      var $this = $(this), data = $this.data("colorselector"), options = $.extend({}, $.fn.colorselector.defaults, $this.data(), typeof option == "object" && option);

      if (!data) {
        $this.data("colorselector", (data = new ColorSelector(this, options)));
      }
      if (typeof option == "string") {
        data[option].apply(data, args);
      }
    });
  };

  $.fn.colorselector.defaults = {
    callback : function(value, name) {
    },
    colorsPerRow : 8
  };

  $.fn.colorselector.Constructor = ColorSelector;

})(jQuery, window, document);
