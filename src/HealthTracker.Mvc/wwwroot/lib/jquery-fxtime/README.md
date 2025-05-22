# jquery-fxtime

### ABOUT

jquery-fxtime is a plugin for jQuery that provides a Firefox-like time
entry element.

Features:

* 12- or 24-hour clock based on the browser's locale
* input keys restricted to legal values
* first empty segment focused on entry
* focus automatically advances to next segment
* may optionally include seconds
* can apply validation (required, minimum, maximum)

Try a simple example here:

https://jsfiddle.net/zookeeper/rv9bafkg/


### INSTALLATION

1. Include the `jquery-fxtime.js` script in your webpage.  In
addition, include jQuery, if it has not already been included.  For
example,

```html
<!-- include jQuery, if your webpage has not already included it -->
<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.4/jquery.min.js"></script>
<!-- include the minified jquery-fxtime.js -->
<script src="https://cdn.jsdelivr.net/gh/RocketMan/jquery-fxtime/dist/jquery-fxtime.min.js"></script>
```

2. On page load in your webpage, invoke $.fn.fxtime on the desired
HTML INPUT elements.  For example, to install fxtime on all INPUT
elements with class 'time':

```javascript
  $().ready(function() {
      $("input.time").fxtime();
  });
```

### USE

fxtime uses a 12- or 24-hour format depending on the browser's locale.
By default, only hours and minutes are displayed.

The following optional attributes are recognized on the INPUT element:

* `step='integer'`
      to display seconds, specify a value less than 60 (e.g., step='1')
* `min='hh:mm[:ss]'`
      minimum time value (in 24-hour format)
* `max='hh:mm[:ss]'`
      maximum time value (in 24-hour format)
* `required`

If 'min' and/or 'max' are specified, validation will be performed
against the time value, and the pseudo-classes `:valid` and `:invalid`
will be set on the element as appropriate.

If 'required' is specified, `:valid` will be set if and only if a valid
time value has been entered.


The following methods are available:

* `$(selector).fxtime('val')` - get 24-hour time value;
      returns null if time is not set
* `$(selector).fxtime('val', value)` - set 24-hour time value;
      value format is hh:mm[:ss], where 0 <= hh <= 23, or null
* `$(selector).fxtime('seg', seg)` - get specified segment value;
      seg:  0 = hours, 1 = minutes, 2 = seconds, 3 = AM/PM
* `$(selector).fxtime('seg', seg, value)` - set specified segment
* `$(selector).fxtime('seg', seg, null)` - clear specified segment
* `$(selector).fxtime('inc', seg)` - increment specified segment
* `$(selector).fxtime('inc', seg, -1)` - decrement specified segment

The element fires the following custom events:

* `segblur` - fires when a segment blurs
      segment number is supplied in the detail.seg property of the event

NB: If you want to be notified of changes to the element, listen for
'change' events.  'change' is fired when a user changes the time value,
before focus is lost.  The element does NOT fire 'input' events.

### KEYBOARD

In addition to numeric time entry, the following keystrokes are
recognised:

* arrow up/down - increment/decrement the current time segment
* `+` / `-` - increment/decrement the current time segment
* tab/shift tab - advance to the next (previous) time segment
* arrow right/left - advance to the next (previous) time segment
* delete/backspace - clear the current time segment
* `A` / `P` - set AM or PM (12 hour mode only)

### LEGAL

**jquery-fxtime** is released under the
[**MIT LICENSE**](https://mit-license.org/)

Firefox is a trademark of the Mozilla Foundation ('Mozilla').  This
work incorporates no source code from Firefox or Mozilla, and is
neither affiliated with nor endorsed by Mozilla.

Copyright &copy; 2022-2024 Jim Mason.
