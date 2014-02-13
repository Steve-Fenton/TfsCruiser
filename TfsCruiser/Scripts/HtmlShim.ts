(function(window, document) {
    var version = '3.6.2pre';
    var options = window.html5 || {};
    var reSkip = /^<|^(?:button|map|select|textarea|object|iframe|option|optgroup)$/i;
    var saveClones = /^(?:a|b|code|div|fieldset|h1|h2|h3|h4|h5|h6|i|label|li|ol|p|q|span|strong|style|table|tbody|td|th|tr|ul)$/i;
    var supportsHtml5Styles;
    var expando = '_html5shiv';
    var expanID = 0;
    var expandoData = {};
    var supportsUnknownElements;
    (function() {
        try {
            var a = document.createElement('a');
            a.innerHTML = '<xyz></xyz>';
            //if the hidden property is implemented we can assume, that the browser supports basic HTML5 Styles
            supportsHtml5Styles = ('hidden' in a);

            supportsUnknownElements = a.childNodes.length == 1 || (function() {
                // assign a false positive if unable to shiv
                (document.createElement)('a');
                var frag = document.createDocumentFragment();
                return (
                  typeof frag.cloneNode == 'undefined' ||
                  typeof frag.createDocumentFragment == 'undefined' ||
                  typeof frag.createElement == 'undefined'
                );
            } ());
        } catch (e) {
        // assign a false positive if detection fails => unable to shiv
            supportsHtml5Styles = true;
            supportsUnknownElements = true;
        }

    } ());

    // Creates a style sheet with the given CSS text and adds it to the document.
    function addStyleSheet(ownerDocument, cssText) {
        var p = ownerDocument.createElement('p'),
            parent = ownerDocument.getElementsByTagName('head')[0] || ownerDocument.documentElement;

        p.innerHTML = 'x<style>' + cssText + '</style>';
        return parent.insertBefore(p.lastChild, parent.firstChild);
    }

    // Returns the value of `html5.elements` as an array.
    function getElements() {
        var elements = html5.elements;
        return typeof elements == 'string' ? elements.split(' ') : elements;
    }

   // Returns the data associated to the given document
    function getExpandoData(ownerDocument) {
        var data = expandoData[ownerDocument[expando]];
        if (!data) {
            data = {};
            expanID++;
            ownerDocument[expando] = expanID;
            expandoData[expanID] = data;
        }
        return data;
    }

    // returns a shived element for the given nodeName and document
    function createElement(nodeName, ownerDocument, data) {
        if (!ownerDocument) {
            ownerDocument = document;
        }
        if (supportsUnknownElements) {
            return ownerDocument.createElement(nodeName);
        }
        if (!data) {
            data = getExpandoData(ownerDocument);
        }
        var node;

        if (data.cache[nodeName]) {
            node = data.cache[nodeName].cloneNode();
        } else if (saveClones.test(nodeName)) {
            node = (data.cache[nodeName] = data.createElem(nodeName)).cloneNode();
        } else {
            node = data.createElem(nodeName);
        }

        return node.canHaveChildren && !reSkip.test(nodeName) ? data.frag.appendChild(node) : node;
    }

    // returns a shived DocumentFragment for the given document
    function createDocumentFragment(ownerDocument, data) {
        if (!ownerDocument) {
            ownerDocument = document;
        }
        if (supportsUnknownElements) {
            return ownerDocument.createDocumentFragment();
        }
        data = data || getExpandoData(ownerDocument);
        var clone = data.frag.cloneNode(),
            i = 0,
            elems = getElements(),
            l = elems.length;
        for (; i < l; i++) {
            clone.createElement(elems[i]);
        }
        return clone;
    }

    // Shivs the `createElement` and `createDocumentFragment` methods of the document.
    function shivMethods(ownerDocument, data) {
        if (!data.cache) {
            data.cache = {};
            data.createElem = ownerDocument.createElement;
            data.createFrag = ownerDocument.createDocumentFragment;
            data.frag = data.createFrag();
        }


        ownerDocument.createElement = function (nodeName) {
            //abort shiv
            if (!html5.shivMethods) {
                return data.createElem(nodeName);
            }
            return createElement(nodeName, ownerDocument, data);
        };

        ownerDocument.createDocumentFragment = Function('h,f', 'return function(){' +
          'var n=f.cloneNode(),c=n.createElement;' +
          'h.shivMethods&&(' +
            // unroll the `createElement` calls
            getElements().join().replace(/\w+/g, function (nodeName) {
                data.createElem(nodeName);
                data.frag.createElement(nodeName);
                return 'c("' + nodeName + '")';
            }) +
          ');return n}'
        )(html5, data.frag);
    }

    // Shivs the given document.
    function shivDocument(ownerDocument) {
        if (!ownerDocument) {
            ownerDocument = document;
        }
        var data = getExpandoData(ownerDocument);

        if (html5.shivCSS && !supportsHtml5Styles && !data.hasCSS) {
            data.hasCSS = !!addStyleSheet(ownerDocument,
              // corrects block display not defined in IE6/7/8/9
              'article,aside,figcaption,figure,footer,header,hgroup,main,nav,section{display:block}' +
              // adds styling not present in IE6/7/8/9
              'mark{background:#FF0;color:#000}'
            );
        }
        if (!supportsUnknownElements) {
            shivMethods(ownerDocument, data);
        }
        return ownerDocument;
    }

    var html5 = {
        'elements': options.elements || 'abbr article aside audio bdi canvas data datalist details figcaption figure footer header hgroup main mark meter nav output progress section summary time video',
        'version': version,
        'shivCSS': (options.shivCSS !== false),
        'supportsUnknownElements': supportsUnknownElements,
        'shivMethods': (options.shivMethods !== false),
        'type': 'default',
        'shivDocument': shivDocument,
        createElement: createElement,
        createDocumentFragment: createDocumentFragment
    };

    // expose html5
    window.html5 = html5;

    // shiv the document
    shivDocument(document);

} (this, document));