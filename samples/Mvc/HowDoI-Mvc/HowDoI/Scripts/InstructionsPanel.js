function toggleInstrux(){
    var flag = true;
    if (document.getElementById('instrux-body').style.display == 'none') flag = false;
    
    if(flag){
        document.getElementById('instrux-body').style.display = 'none';
        document.getElementById('showhide-button').value = '+';
    }else{
        document.getElementById('instrux-body').style.display = 'block';
        document.getElementById('showhide-button').value = '-';
    }
}