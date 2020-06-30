//const serverURL = "http://127.0.0.1:8000/SERVER"
const serverURL = "https://party-game-mobile.herokuapp.com/SERVER"


function GetRequest(uri, func) {
	var xmlHttp = new XMLHttpRequest();
			
	xmlHttp.onreadystatechange = func;
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
			func();
		}
	}
	xmlHttp.send(toSend);
}