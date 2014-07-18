(function($)
{
	
	$('[class*="filter"][data-filter] a').on('click', function(e) {
		e.preventDefault();
	});

	$('.mixitup').each(function(){
		$(this).mixitup({ 
			showOnLoad: $(this).attr('data-show-default') || 'mixit-filter-1',
			layoutMode: $(this).attr('data-layout-mode') || 'grid',
			listEffects: $(this).attr('data-list-effects') ? $(this).attr('data-list-effects').split(' ') : ['fade','rotateX'],
			targetSelector: $(this).attr('data-target-selector') || '.mix',
		    filterSelector: $(this).attr('data-filter-selector') || '.filter'
		});
	});

})(jQuery);