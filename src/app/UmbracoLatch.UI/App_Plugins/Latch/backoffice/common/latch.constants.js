(function() {
    'use strict';

    angular
        .module('umbraco')
        .constant('Latch.Constants', {
            VERSION: '1.0.0',
            BUTTON_STATE: {
                INIT: 'init',
                BUSY: 'busy',
                SUCCESS: 'success',
                ERROR: 'error'
            },
            OPERATION_TYPES: {
                LOGIN: { id: 'login', actions: [] },
                CONTENT: { id: 'content', actions: ['delete', 'publish', 'unpublish'] },
                MEDIA: { id: 'media', actions: ['delete'] },
                DICTIONARY: { id: 'dictionary', actions: ['delete'] }
            }
        });

})();
