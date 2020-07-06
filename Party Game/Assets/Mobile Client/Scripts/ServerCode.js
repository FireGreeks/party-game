const serverURL = "http://127.0.0.1:8000/SERVER"
//const serverURL = "https://party-game-mobile.herokuapp.com/SERVER"

const CDN_URL = "https://raw.githack.com/FireGreeks/party-game/master/Party%20Game/Assets/Mobile%20Client";


function GetRequest(uri, func) {
	var xmlHttp = new XMLHttpRequest();
			
	xmlHttp.onreadystatechange = function() {
		if (this.readyState == 4 && this.status == 200) {
			func(this)
		}
	}
	xmlHttp.open( "GET", uri, true ); // false for synchronous request
	xmlHttp.send(null);
}
		
function PostRequest(uri, toSend, func) {
	var xmlHttp = new XMLHttpRequest();
	xmlHttp.open("POST", uri, true);

	xmlHttp.setRequestHeader("Content-Type", "application/json");
	//xmlHttp.setRequestHeader("Length", (new TextEncoder().encode(toSend)).length)
			
	xmlHttp.onreadystatechange = function() {
		if (this.readyState === XMLHttpRequest.DONE && this.status === 200) {
			func(this);
		}
	}
	xmlHttp.send(toSend);
}

function GetRoomPath() {
	if (window.location.href.includes(".com"))
		return serverURL + "/" + window.location.href.substr(window.location.href.length - 4);
	else
		return serverURL + "/H8JK"
}