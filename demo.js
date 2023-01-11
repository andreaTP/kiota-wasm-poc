import { generateOnElement } from './main.js'

var button = document.createElement('button');
document.body.appendChild(button);
button.innerText = "Generate";
button.onclick = function() {
  var element = document.createElement('a');
  document.body.appendChild(element);
  generateOnElement(
      element,
      document.getElementById("url").value,
      document.getElementById("language").value,
      document.getElementById("client").value,
      document.getElementById("package").value
  );
  element.onclick = function () {
    document.body.removeChild(element);
  };  
}
