function scrollTo(e, t) {
    var n = t || -1,
        o = document.getElementById(e),
        i = window.scrollY ? window.scrollY : 0;
    animate(document.scrollingElement || document.documentElement, "scrollTop", "", i, o.offsetTop - n, 400, !0)
  }
  
  function animate(e, t, n, o, i, r, l) {
    if (e) {
        var c = (new Date).getTime(),
            s = setInterval(function () {
                var a = Math.min(1, ((new Date).getTime() - c) / r);
                l ? e[t] = o + a * (i - o) + n : e.style[t] = o + a * (i - o) + n, 1 === a && clearInterval(s)
            }, 1);
        l ? e[t] = o + n : e.style[t] = o + n
    }
  }
  
  
  document.querySelectorAll("a").forEach(function (e) {
    try {
        "#" == e.getAttribute("href").substr(0, 1) && "#_" != e.getAttribute("href").substr(0, 2) && e.addEventListener("click", function (t) {
            return t.preventDefault(), scrollTo(e.getAttribute("href").replace("#", ""), 0), document.querySelector(e.getAttribute("href")).querySelectorAll("*").forEach(function(t){
                let p = document.querySelector(e.getAttribute("href"));
                if(p.style.display == "none"){p.style.display = "";}
              t.classList.add("build-in-up");
            });
        }) && console.log(t);
    } catch (e) { 
      // console.log(e); 
    }
  });
  
  
  try{
      let login = document.getElementById("loginForm_");
      let register = document.getElementById("registerForm_");
    document.getElementById("Register").addEventListener("click", function(t){
      login.style.display = "none";
      register.style.display = "";
    });
  
    document.getElementById("SignIn").addEventListener("click", function(t){
      register.style.display = "none";
      login.style.display = "";
    });
  
  
  }catch(e){
      //
  }


  document.querySelectorAll('iframe').forEach(function(t){
    t.addEventListener("load", function() {
      var h = t.contentDocument.body.scrollHeight;
      console.log('working on: ' + h);
      t.style.height = h+20 + 'px';
    })
  })

    // window.addEventListener('message', function(e) {
    //   // message passed can be accessed in "data" attribute of the event object
    //   var scroll_height = e.data;
    //   console.log(e.data);
    //   // document.getElementById('iframe-container').style.height = scroll_height + 'px'; 
    // } , false);