(function($)
{
	// HTML Helper
    var documentHtml = function(html)
    {
        var result = String(html)
            .replace(/<\!DOCTYPE[^>]*>/i, '')
            .replace(/<(html|head|body|title|meta|script)([\s\>])/gi,'<div class="document-$1"$2')
            .replace(/<\/(html|head|body|title|meta|script)\>/gi,'</div>');
            
        return $.trim(result);
    };

    new $.LazyJaxDavis(function(router)
	{
		router.option({
		    davis: {
				linkSelector: 'a:not([href^="#"]):not([href=""]):not(.no-ajaxify):not([data-toggle]):not([target="_blank"])'
	    	}
	  	});

    	var $root = $('#content'),
    		$menu_selectors = App.Config.ajaxify_menu_selectors;

    	router.bind('everyfetchstart', function(page)
    	{
      		window.scrollTo(0, 0);
    	});

    	router.bind('everyfetchsuccess', function(page)
    	{
    		var page_html = $('<div>').html(page._text);

      		$newcontent = $(page_html.find('#content').html());
      		$root.empty().append($newcontent);

      		$.each($menu_selectors, function(i,n){
      			var $menu = $(n);
      			$menu.html(page_html.find(n).html());
      		});

      		var $data = $(documentHtml(page._text)),
				$scripts = $data.find('.document-script'),
				$scriptsText = $scripts.filter('[data-id="App.Scripts"]').text(),
				$configText = $scripts.filter('[data-id="App.Config"]').text(),
				$u = Math.round(new Date().getTime() + (Math.random() * 100)),
				$ignore = [ "ajaxify.init.js", "pace.init.js" ],
				$ignoreLoaded = [ "initGoogleMaps" ],
				$loadAlways = [ "maps-google.init.js" ];

			$.globalEval($scriptsText);
			$.globalEval($configText);

			// cleanup
			$(window).off('load');

			// datatables cleanup
			if ($('.FixedHeader_Cloned').length)
				$('.FixedHeader_Cloned').remove();

			if (App.Config.ajaxify_layout_app && !$('html.ie').length)
				$('html').addClass('app');
			if (!App.Config.ajaxify_layout_app)
				$('html').removeClass('app');

			$script.ready('bundle' + $u, function()
			{
				if (typeof animations !== 'undefined')
		    		animateElements();

		    	page.trigger('pageready');
		    	$(window).trigger('load');

		    	if (typeof resizeNiceScroll !== 'undefined')
		    		setTimeout(function(){ resizeNiceScroll(); }, 1000);
			});

			// loading plugins 
			// - only if they plugins are not loaded already and;
			// - if the plugins don't match the ignore list;
			var plugins_dependency = App.Scripts.plugins_dependency.filter(function(item)
				{ 
					var ignore = false,
						loadAlways = false;

					$.each($ignore, function(i,n){
						ignore = ignore || item.indexOf(n) > -1;
					});
					ignore = ignore || $('[src*="' + item + '"]').length > 0;

					$.each($loadAlways, function(i,n){
						loadAlways = loadAlways || item.indexOf(n) > -1;
					});
					
					// console.log(item);
					// console.log('Load:' + loadAlways);
					// console.log('Ignore:' + ignore);

					return typeof item == 'string' && (ignore !== true || loadAlways === true);
				}),

				plugins = App.Scripts.plugins.filter(function(item)
				{ 
					var ignore = false;
					$.each($ignore, function(i,n){
						ignore = ignore || item.indexOf(n) > -1;
					});
					ignore = ignore || $('[src*="' + item + '"]').length;
					return typeof item == 'string' && ignore !== true;
				}),

				// the bundle package / initialization scripts 
				// - will be loaded every time (except if matching the ignore list)
				bundle = App.Scripts.bundle.filter(function(item)
				{ 
					var ignore = false;
					$.each($ignore, function(i,n){
						ignore = ignore || item.indexOf(n) > -1;
					});
					return typeof item == 'string' && ignore !== true;
				});

			// remove existing scripts
			$.each(plugins_dependency, function(i,n){ $('[src*="' + n + '"]').remove(); });
			$.each(bundle, function(i,n){ $('[src*="' + n + '"]').remove(); });

			// append unique id to the scripts url
			plugins_dependency = plugins_dependency.map(function(t){ 
				return t + ( t.indexOf("?") > -1 ? "&" : "?" ) + $u;
			});
			bundle = bundle.map(function(t){ 
				return t + ( t.indexOf("?") > -1 ? "&" : "?" ) + $u;
			});

			// load scripts
			if (plugins_dependency.length)
			{
				// console.log('-----');
				// console.log('plugins_dependency');
				// console.log(plugins_dependency);
				// console.log('-----');
			
				$script(plugins_dependency, 'plugins_dependency' + $u, function(){
					$script(plugins, 'plugins' + $u, function(){
						$script(bundle, 'bundle' + $u);
					});
				});
			}
			else
			{
				$script(plugins, 'plugins' + $u, function(){
					$script(bundle, 'bundle' + $u);
				});
			}

    	});

    	router.bind('everyfetchfail', function()
    	{
      		alert('ajax error!');
      		$root.css('opacity', 1);
    	});
  	});

})(jQuery);