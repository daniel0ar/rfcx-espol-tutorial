$.fn.pageMe = function(opts){
    
    var $this = this,
        defaults = {
            perPage: 10,
            showPrevNext: false,
            hidePageNumbers: false
        },
        settings = $.extend(defaults, opts);
 
    var listElement = $this;

    var perPage = settings.perPage;
    
    var children = listElement.children();

        
    /*   BEGIN: Delete audios out of date of paged table   */

    var audiosOutOfDate = 0;

    for(var i=0; i< children.length; i++){
        var lengthChildren = (children[i].innerHTML);
        if((lengthChildren.length) == 19){
            audiosOutOfDate ++;
        }    
    }   
 
    while(audiosOutOfDate > 0){
        for(var i=0; i< children.length; i++){
            var l_children = (children[i].innerHTML);
            if((l_children.length) == 19){
                children.splice(i, 1);
            }    
        }
        audiosOutOfDate--;
    }    
 
    /*  END: Delete audios out of date of paged table   */
    
    var pager = $('.pager');
     

    if (typeof settings.childSelector!="undefined") {
        children = listElement.find(settings.childSelector);
    }
    
    if (typeof settings.pagerSelector!="undefined") {
        pager = $(settings.pagerSelector);
    }
    
    var numItems = children.size();

    var numPages = Math.ceil(numItems/perPage);
    
    pager.data("curr",0);
    
    
    if (settings.showPrevNext){
        $('<li><a href="#" class="prev_link">«</a></li>').appendTo(pager);;
    }
    
    /* BEGIN: Create buttons with paging numbers. */

    var curr = 0;

    while(numPages > curr && (settings.hidePageNumbers==false)){
        $('<li><a href="#" class="page_link">'+(curr+1)+'</a></li>').appendTo(pager);
        curr++;
    }
    
    /* END: Create buttons with paging numbers. */
    
    if (settings.showPrevNext){
        $('<li><a href="#" class="next_link">»</a></li>').appendTo(pager);
    }
    
    pager.find('.page_link:first').addClass('active');
    pager.find('.prev_link').hide();
    if (numPages <= 1) {
        pager.find('.next_link').hide();
    }
  	pager.children().eq(1).addClass("active");
    
    children.hide();
    
    children.slice(0, perPage).show();
    
    
    pager.find('li .page_link').click(function(){
        var clickedPage = $(this).html().valueOf()-1;
        goTo(clickedPage,perPage);
        return false;
    });
    pager.find('li .prev_link').click(function(){
        previous();
        return false;
    });
    pager.find('li .next_link').click(function(){
        next();
        return false;
    });
    
    function previous(){
        var goToPage = parseInt(pager.data("curr")) - 1;
        goTo(goToPage);
    }
     
    function next(){
        goToPage = parseInt(pager.data("curr")) + 1;
        goTo(goToPage);
    }
    
    function goTo(page){
        var startAt = page * perPage,
            endOn = startAt + perPage;

        children.css('display','none').slice(startAt, endOn).show();
        
        if (page>=1) {
            pager.find('.prev_link').show();
        }
        else {
            pager.find('.prev_link').hide();
        }
        
        if (page<(numPages-1)) {
            pager.find('.next_link').show();
        }
        else {
            pager.find('.next_link').hide();
        }
        
        pager.data("curr",page);
      	pager.children().removeClass("active");
        pager.children().eq(page+1).addClass("active");
    
    }

};

$(document).ready(function(){

    $('#myTable-page').pageMe({pagerSelector:'#myPager',showPrevNext:true,hidePageNumbers:false,perPage: 10});

});


