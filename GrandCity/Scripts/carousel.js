/**
 * jQuery plugin for an infinite carousel
 */
/* define $ as jQuery just in case */
(function ($) {
	/* circular carousel - my custom plugin */
	$.fn.carousel = function () {
		/* set static vars */
		var carousel = this;
		var slide_window = carousel.find('.slide_window');
		var slides = carousel.find('.slide');
		var slide_count = slides.length;
		var first_slide = carousel.find('.slide:eq(0)');
		var last_slide = carousel.find('.slide:eq(' + parseInt(slide_count - 1) + ')');
		var slide_w = first_slide.outerWidth();
		var slide_h = first_slide.outerHeight();
		var slide_width = slide_w > 0 ? parseInt(slide_w) + 'px' : 'auto';
		var slide_height = slide_w > 0 ? parseInt(slide_h) + 'px' : 'auto';

		/* set the css */
		carousel.hide();
		$(window).load(function () {
			carousel.css({
				'clear': 'both',
				'overflow': 'hidden',
				'position': 'relative'
			});
			slide_window.css({
				'height': slide_height,
				'width': slide_width,
				'overflow': 'hidden'
			});
			carousel.find('.slide').css({
				'position': 'absolute',
				'overflow': 'hidden'
			});

			/* if there is only one slide - hide the controls */
			if (slide_count < 2) {
				carousel.find('.control').hide();
			} else if (slide_count == 2) {
				/* if only 2 - duplicate the slides on either side so the animation is smooth */
				first_slide.clone().appendTo(slide_window);
				last_slide.clone().prependTo(slide_window);
			} else {
				/**
				 * add the last to the beginning and then remove the last
				 * - this is to account for the initial negative offset so that the first item is actually the "second" item viewed with an offset of 0
				 */
				last_slide.clone().prependTo(slide_window);
				last_slide.remove();
			}

			/* offset the slides */
			offset_slides(false);

			/* show the carousel when everything is loaded */
			carousel.show();
		});

		/* navigation */
		carousel.on('click', '.control', function (e) {

			/* set the vars */
			var anim_active = '1';
			var slides = carousel.find('.slide');
			var slide_count = parseInt(slides.length);

			/* if next or previous arrow clicked */
			var trigger = $(this);
			set_slides(trigger);
			offset_slides(true);
			e.preventDefault();

		});

		function set_slides(trigger) {
			/* set the vars */
			var slides = carousel.find('.slide');
			var slide_count = parseInt(slides.length);

			if (trigger.hasClass('next')) {
				/* grab the first slide */
				var first_slide = carousel.find('.slide:eq(0)');

				/* clone the next slide and add it to the end of the slides, but set the offset to positive first so the animation doesn't go over the other slides */
				first_slide.clone().css({
					'left': parseInt(slide_w * slide_count) + 'px'
				}).appendTo(slide_window);
				first_slide.remove();
			} else if (trigger.hasClass('prev')) {
				/* grab the last slide */
				var last_slide = carousel.find('.slide:eq(' + parseInt(slide_count - 1) + ')');

				/* clone the last slide and add it to the beginning of the slides, but set the offset to negative first so the animation doesn't go over the other slides */
				last_slide.clone().css({
					'left': '-' + slide_w + 'px'
				}).prependTo(slide_window);
				last_slide.remove();
			}
		}

		/* offset the slides - set the argument to true to animate it for scrolling effect */
		function offset_slides(animate) {
			/* only if there are 2 or more slides */
			if (slide_count > 1) {
				/* set the offset for each slide */
				var slides = carousel.find('.slide');
				$.each(slides, function (i) {
					var slide_offset = parseInt(slide_w * (i - 1));
					if (animate === true) {
						carousel.find('.slide:eq(' + i + ')').animate({
							'left': slide_offset + 'px',
							'width': slide_width
						}, {
							queue: false,
							duration: 200
						});
					} else {
						carousel.find('.slide:eq(' + i + ')').css({
							'left': slide_offset + 'px',
							'width': slide_width
						});
					}
				});
			}
		}
	}

})(jQuery);
