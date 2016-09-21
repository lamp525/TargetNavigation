// QQ表情插件
(function($){  
	$.fn.qqFace = function(options){
		var defaults = {
			id : 'facebox',
			path : 'face/',
			assign : 'content',
			tip : 'em_',
            show:'editable'
		};
		var option = $.extend(defaults, options);
		var assign = $('#'+option.assign);
		var id = option.id;
		var path = option.path;
		var tip = option.tip;
		if(assign.length<=0){
			alert('缺少表情赋值对象。');
			return false;
		}

		$(this).click(function(e){
			var strFace, labFace;
			if($('#'+id).length<=0){
				strFace = '<div id="'+id+'" style="position:absolute;display:none;z-index:1000;" class="qqFace">' +
							  '<table border="0" cellspacing="0" cellpadding="0"><tr>';
				for(var i=1; i<=75; i++){
					labFace = '['+tip+i+']';
					strFace += '<td><img src="'+path+i+'.gif" onclick="$(\'#'+option.assign+'\').setCaret();$(\'#'+option.assign+'\').insertAtCaret(\'' + labFace + '\',\'#'+option.show+'\');" /></td>';
					if( i % 15 == 0 ) strFace += '</tr><tr>';
				}
				strFace += '</tr></table></div>';
			}
			$(this).parent().append(strFace);
			var offset = $(this).position();
			var top = offset.top + $(this).outerHeight();
			$('#'+id).css('top',top);
			$('#'+id).css('left',offset.left);
			$('#'+id).show();
			e.stopPropagation();
		});

        function replace_em(str) {
            str = str.replace(/\</g, '&lt;');
            str = str.replace(/\>/g, '&gt;');
            str = str.replace(/\n/g, '<br/>');
            str = str.replace(/\[em_([0-9]*)\]/g, '<img src="../chat/arclist/$1.gif" border="0" />');
            return str;
        }




        $(document).click(function(){
			$('#'+id).hide();
			$('#'+id).remove();
		});
	};

})(jQuery);

jQuery.extend({ 
unselectContents: function(){ 
	if(window.getSelection) 
		window.getSelection().removeAllRanges(); 
	else if(document.selection) 
		document.selection.empty(); 
	} 
}); 
jQuery.fn.extend({ 
	selectContents: function(){ 
		$(this).each(function(i){ 
			var node = this; 
			var selection, range, doc, win; 
			if ((doc = node.ownerDocument) && (win = doc.defaultView) && typeof win.getSelection != 'undefined' && typeof doc.createRange != 'undefined' && (selection = window.getSelection()) && typeof selection.removeAllRanges != 'undefined'){ 
				range = doc.createRange(); 
				range.selectNode(node); 
				if(i == 0){ 
					selection.removeAllRanges(); 
				} 
				selection.addRange(range); 
			} else if (document.body && typeof document.body.createTextRange != 'undefined' && (range = document.body.createTextRange())){ 
				range.moveToElementText(node); 
				range.select(); 
			} 
		}); 
	}, 

	setCaret: function(){ 
		if(!$.browser.msie) return; 
		var initSetCaret = function(){ 
			var textObj = $(this).get(0); 
			textObj.caretPos = document.selection.createRange().duplicate(); 
		}; 
		$(this).click(initSetCaret).select(initSetCaret).keyup(initSetCaret); 
	}, 

	insertAtCaret: function(textFeildValue,show){
		var textObj = $(this).get(0);
        textObj = $(textObj)
		if(document.all && textObj.createTextRange && textObj.caretPos){
			var caretPos=textObj.caretPos; 
			caretPos.text = caretPos.text.charAt(caretPos.text.length-1) == '' ? 
			textFeildValue+'' : textFeildValue;
		} else if(textObj.setSelectionRange){
		    var rangeStart = textObj.selectionStart;
		    console.log('range', rangeStart)
			var rangeEnd=textObj.selectionEnd; 
			var tempStr1=textObj.value.substring(0,rangeStart); 
			var tempStr2=textObj.value.substring(rangeEnd);
			//textObj.value=tempStr1+textFeildValue+tempStr2;
            var temp = tempStr1+textFeildValue+tempStr2;
            temp = temp.replace(/\</g, '&lt;');
            temp = temp.replace(/\>/g, '&gt;');
            temp = temp.replace(/\n/g, '<br/>');
            temp = temp.replace(/\[em_([0-9]*)\]/g, '<img src="../Images/chat/arclist/$1.gif" border="0" />');
            textObj.html(temp);
			textObj.focus();
			var len=textFeildValue.length;
			textObj.setSelectionRange(rangeStart+len,rangeStart+len);
			textObj.blur(); 
		}else{
            //textObj.focus();
		    var temp = textFeildValue;
		    console.log('textFeildValu1111e', textFeildValue)
		    textFeildValue = textFeildValue.replace(/\</g, '&lt;');
		    textFeildValue = textFeildValue.replace(/\>/g, '&gt;');
		    textFeildValue = textFeildValue.replace(/\n/g, '<br/>');
		    textFeildValue = textFeildValue.replace(/\[em_([0-9]*)\]/g, '<img src="../Images/chat/arclist/$1.gif" border="0" em=' + temp + ' class="defaultNot" />');
		    //var selection = window.getSelection ? window.getSelection() : document.selection,
            //range = selection.createRange ? selection.createRange() : selection.getRangeAt(0);
		    //var start = selection.anchorOffset;
		    //var end = selection.focusOffset;
		    //var node = $(textFeildValue).get(0)
		    //range.insertNode(node);
		    //selection.addRange(range);
		    //获取光标位置结束



		    var range, node;
		    if (!textObj.hasfocus) {
		        textObj.focus();
		    }
		    if (window.getSelection && window.getSelection().getRangeAt) {
		        range = window.getSelection().getRangeAt(0);
		        range.collapse(false);
		        node = range.createContextualFragment(textFeildValue);
		        var c = node.lastChild;
		        range.insertNode(node);
		        if (c) {
		            range.setEndAfter(c);
		            range.setStartAfter(c)
		        }
		        var j = window.getSelection();
		        j.removeAllRanges();
		        j.addRange(range);

		    } else if (document.selection && document.selection.createRange) {
		        document.selection.createRange().pasteHTML(textFeildValue);
		    }



		    //textObj.value+=textFeildValue;
		    
            //var value = textObj.html().replace('<br>', '') + textFeildValue;
            //console.log('vv', value);
            //textObj.html(value);
            

		}

	}

});