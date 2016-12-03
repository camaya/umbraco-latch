module.exports = function(grunt) {

    var pkg = {
        name: 'Umbraco Latch',
        alias: 'UmbracoLatch',
        version: '1.0.0',
        url: 'https://github.com/camaya/umbraco-latch',
        license: 'LGPL 2.1',
        licenseUrl: 'https://opensource.org/licenses/LGPL-2.1',
        author: 'Cristhian Amaya',
        authorUrl: 'http://camaya.co',
        readme: 'Umbraco Latch integrates the Latch service within the backoffice, allowing you to protect different operations in the CMS.'
    };

    grunt.initConfig({
        pkg: pkg,
        clean: {
            files: [
                'releases/files/*'
            ]
        },
        copy: {
            main: {
                files: [
                    {
                        expand: true,
                        cwd: 'src/app/UmbracoLatch.Core/bin/Release/',
                        src: ['UmbracoLatch.Core.dll'],
                        dest: 'releases/files/bin/'
                    },
                    {
                        expand: true,
                        cwd: 'lib/LatchSDK/',
                        src: ['LatchSDK.dll'],
                        dest: 'releases/files/bin/'
                    },
                    {
                        expand: true,
                        cwd: 'src/app/UmbracoLatch.UI/App_Plugins/Latch',
                        src: ['**'],
                        dest: 'releases/files/App_Plugins/Latch'
                    },
                    {
                        expand: true,
                        cwd: 'src/app/UmbracoLatch.UI/App_Plugins/Latch/assets/',
                        src: ['umbracolatch-icon.png'],
                        dest: 'releases/files/Umbraco/Images/Tray/'
                    }
                ]
            }
        },
        umbracoPackage: {
            main: {
                src: 'releases/files/',
                dest: 'releases/umbraco',
                options: {
                    name: pkg.name,
                    version: pkg.version,
                    url: pkg.url,
                    license: pkg.license,
                    licenseUrl: pkg.licenseUrl,
                    author: pkg.author,
                    authorUrl: pkg.authorUrl,
                    readme: pkg.readme,
                    outputName: pkg.alias + '-' + pkg.version + '.zip'
                }
            }
        }
    });

    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-umbraco-package');

    grunt.registerTask('dist', ['clean', 'copy', 'umbracoPackage']);
    grunt.registerTask('default', ['dist']);

};
