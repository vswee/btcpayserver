function copyToClipboard(e, t){
  var textarea = document.createElement('textarea');
  textarea.value = t.innerHTML;
  document.body.appendChild(textarea);
  textarea.focus();
  textarea.select();
  var c = document.execCommand('copy');
  var copied = document.createElement('div');
  copied.classList.add('free-hover-alert');
  copied.style.top = e.pageY-40 + 'px';
  copied.style.left = e.pageX+20 + 'px';
  copied.innerHTML = "copied to clipboard";
  document.body.appendChild(copied);
  setTimeout(function(){document.body.removeChild(copied);},1500);
  document.body.removeChild(textarea);
}

// Send message to the top window (parent) at 500ms interval
// setInterval(function() {
  // first parameter is the message to be passed
  // second paramter is the domain of the parent 
  // in this case "*" has been used for demo sake (denoting no preference)
  // in production always pass the target domain for which the message is intended 
  // window.top.postMessage(document.body.scrollHeight, "*");
// }, 500); 