//
// Azurea with Mobile Web Js with App
// By. Itsurea
//

//원 저작자가 제가아닙니다.
//고로 배포하시면 원저작자가 싫어할 가능성이 있습니다.
//하지만 너무 잘만든 스크립트라서 수정해서 사용했습니다!
//뭔가 안된다! 하면 err.txt파일이 생길텐데 그걸 @tensimiku로 보내주세요!
//配布するのはかまわないが元の製作者がいやがる可能性があります。
//上の件は適宜処理してください。
//エラーが発生したときは＠tensimikuにお知らせくたさい！



var otherchk = System.settings.getValue('user.API', 'Status');
var mobilechk = System.settings.getValue('user.Mobile', 'Status');

if (otherchk == 0 | !otherchk) {
	System.showNotice('현재 Azurea 트윗을 사용하고 있습니다.');
} else {
	System.showNotice('현재 다른 API를 사용하고 있습니다.');
}
function PreSendUpdateStatus(status) {
	if (status.text.match('Hash(\\d+)=(.*)')) {
		return;
	}
	if (status.text.match('Fillter=(.*)')) {
		return;
	}
	if (otherchk == 0 | !otherchk) {
		return;
	}
	if(mobilechk == 1 | !mobilechk) {
		return;
	}
	var cont = '"'+ encodeURIComponent(status.text) +'"';
	var args = (status.in_reply_to_status_id ? status.in_reply_to_status_id : '') + ' ' + cont;
	TextArea.text = '';
	TextArea.hide();
	System.launchApplication(System.applicationPath.replace(/[^\\]+$/, '\Scripts\\tenmiktwit.exe'),args,0);
	return true;
}

function changetwt() {
	if (!otherchk | otherchk == 0) {
		System.settings.setValue('user.API', 'Status', '1');
		System.settings.reconfigure();
		otherchk = "1";
		System.alert('다른 API로 트윗합니다.');
	}
	else{
		System.settings.setValue('user.API', 'Status', '0');
		System.settings.reconfigure();
		otherchk = 0;
		System.alert('기본 Azurea 트윗을 사용합니다..');
	}
}

TwitterService.addEventListener('preSendUpdateStatus', PreSendUpdateStatus);
System.addKeyBindingHandler('R'.charCodeAt(0), 2, changetwt);