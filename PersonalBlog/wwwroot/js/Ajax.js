
var Ajax = {
  get: function (url, callback) {
    var obj = null; //得到xhr对象
    if (XMLHttpRequest) {
      obj = new XMLHttpRequest();
    } else {
      obj = new ActiveXObject("Microsoft.XMLHTTP");
    }
    obj.open("GET", url, true);
    obj.onreadystatechange = function () {
      if (obj.readyState === 4 && obj.status === 200 || obj.status === 304) {
        callback.call(this, obj.responseText);
      }
    }
    obj.send();
  },
  post: function (url, data, callback) {
    var obj = null; //得到xhr对象
    if (XMLHttpRequest) {
      obj = new XMLHttpRequest();
    } else {
      obj = new ActiveXObject("Microsoft.XMLHTTP");
    }
    obj.open("POST", url, true);
    obj.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    obj.onreadystatechange = () => {
      if (obj.readyState === 4 && obj.status === 200 || obj.status === 304) {
        callback.call(this, obj.responseText);
      }
    }
    obj.send(data);
  }
}