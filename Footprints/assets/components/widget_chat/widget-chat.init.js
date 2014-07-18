$(function()
{	
	/*
	 * Chat widget
	 */
	if ($('.widget-chat').length)
	{
		$('.widget-chat form').submit(function(e)
		{
			e.preventDefault();
			
			var direction = $(this).parents('.widget-chat').find('.media:first blockquote').is('.pull-right') ? 'left' : 'right';
			var media = $(this).parents('.widget-chat').find('.media:first').clone();
			var message = $(this).find('[name="message"]');
			
			// prepare media
			media.hide();
			media.find('small.author a.strong').text('Awesome');
			
			// apply direction
			media.removeClass('right').addClass(direction);
			media.find('blockquote').attr('class', '').addClass('pull-' + direction);
			media.find('.media-object').removeClass('pull-left pull-right').addClass('pull-' + direction);
			
			// apply message
			media.find('blockquote p').text(message.val());
			
			// reset input
			message.val('');
			
			var p = $(this).parents('.widget-chat:first');

			// jump slimScroll to top
			p.find('.slim-scroll').slimScroll({ scrollTo: '0' });
			
			// insert media in the conversation
			p.find('.chat-items').prepend(media).find('.media:hidden').slideDown();
		});
	}
	
});