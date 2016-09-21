function fnAjaxSectProClear () {
	// 筛选里的部门、项目数据清空
	$('.sectPro ul').remove();
}
/* 筛选里的部门数据导入 开始*/
function fnAjaxSectionData () {
	$.ajax({
		type: "post",
		url: "../../testSL/data/Plan/plan_sectionData.json",
		dataType: "json",
		data:{
			//count:5
		},
		success:rsHandler(function(data){
			fnsectProAdd (data,0);
		})
	});
}
/* 筛选里的部门数据导入 结束*/
/* 筛选里的项目数据导入 开始*/
function fnAjaxProjectData () {
	$.ajax({
		type: "post",
		url: "../../testSL/data/Plan/plan_projectData.json",
		dataType: "json",
		data:{
			//count:5
		},
		success:rsHandler(function(data){
			fnsectProAdd (data,1);
		})
	});
}
/* 筛选里的项目数据导入 结束*/
/* 筛选里的部门项目结构导入 开始*/
function fnaddUl (thisData,nums) {
	for ( var num in thisData ) {
		if ( num!='shuffle' ) {
			if ( num!=0 ) {
				var spanTerm = $('.sectPro ul li input[type="checkbox"]').next();
				var spanLength = spanTerm.length;
				for ( var i=0;i<spanLength;i++ ) {
					if ( parseInt(spanTerm.eq(i).attr('term'))==thisData[0].id ) {
						$('.nowSpan').removeClass('nowSpan');
						spanTerm.eq(i).parent().parent().prev('span').addClass('nowSpan');
						break;
					}
				}
			}
			$('.nowSpan').after(
			'<ul style="display:none;">'+
			'<li><span class="arrowSolidRCom"></span>'+
			'<input type="checkbox" />'+
			'<span class="nowSpan" term="'+
			thisData[num].id+
			'">'+
			thisData[num].name+
			'</span>'+
			'</li></ul>').removeClass('nowSpan'); 
			if ( thisData[num].children ) {
				fnaddUl(thisData[num].children,nums);
			}
		}
	}
}
function fnsectProAdd (datas,num) {
	$('.nowSpan').removeClass('nowSpan');
	for ( var nums in datas ) {
		if ( nums!='shuffle' ) {
			$('.sectPro:eq('+num+')').append(
			'<ul class="firUl" style="display:none;">'+
			'<li><span class="arrowSolidRCom"></span>'+
			'<input type="checkbox" />'+
			'<span class="nowSpan" term="'+
			datas[nums].id+
			'">'+
			datas[nums].name+
			'</span>'+
			'</li></ul>'); 
			if ( datas[nums].children ) {
				fnaddUl(datas[nums].children,nums);
				$('.nowSpan').removeClass('nowSpan');
			}
		}
	}
	if ( num==1 ) {
		// 重新加载一次部门和项目的箭头展开，不然操作不成功
		fnSPMArrowsBB ();
		//重新加载一次部门和项目的勾选，不然操作不成功
		fnSectProC ();
		//重新加载一次关闭X，不然操作不成功
		fnCloseWC ();
	}
}
/* 筛选里的部门项目结构导入 结束*/
