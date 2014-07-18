(function($)
{

	function showChatNotification() {
		setTimeout(function(){
			notification.html('New message from <strong>Mr.Awesome</strong>').fadeIn();
		}, 5000);
	}

	if (typeof $.fn.sidr !== 'undefined')
	{
		$('.navbar.main .btn-navbar.btn-open-right').sidr({
			name: 'menu-right',
			side: 'right',
			onOpen: function(){
				setTimeout(function(){
					$('#chat-notification').fadeOut();
				}, 500);
				resizeNiceScroll();
			},
			onClose: function(){
				showChatNotification();
			}
		});
		$('body').on('click', '[data-toggle="sidr-close"]', function(e){
			e.preventDefault();
			$.sidr('close', $(this).data('menu'));
		})
		.on('click', '[data-toggle="sidr-open"]', function(e){
			e.preventDefault();
			$.sidr('open', $(this).data('menu'));
		});
		var notification = $('<div id="chat-notification"></div>').hide().attr('data-menu', 'menu-right').attr('data-toggle', 'sidr-open').appendTo($('.navbar.main .btn-navbar.btn-open-right').parent());
		showChatNotification();
	}
	
})(jQuery);