paceOptions = {
	document: false,
	target: '.paceExample'
};
(function($)
{
	$(function()
	{

		Pace.on('hide', function()
		{
			setTimeout(function()
			{
				Pace.restart();
			}, 1500);
		});

	});
})(jQuery);