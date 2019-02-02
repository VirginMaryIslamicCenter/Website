/// <binding ProjectOpened='1-RunAndWatch' />
"use strict";


// configuration
var gulp = require('gulp'),
    expect = require('gulp-expect-file'),
    concat = require('gulp-concat'),
    uglify = require('gulp-uglify'),
    cssmin = require('gulp-cssmin'),
    sass = require('gulp-sass'),
    del = require('del'),
    gulpif = require('gulp-if'),
    copy = require('gulp-copy'),
    bundleConfig = require('./bundleconfig.json');

// paths configuration
var basePath = './wwwroot/';
var paths = {
    base: basePath,
    css: basePath + 'css/',
    cssFiles: basePath + 'css/**/*.css',
    sass: basePath + 'sass/',
    sassFiles: basePath + 'sass/**/*.scss',
    js: basePath + 'js/',
    jsMinFiles: basePath + 'js/**/*.js',
    jsFiles: basePath + 'source-js/**/*.js',
    webfonts: basePath + 'webfonts/'
};

// regex for parsing bundleconfig.json
var regex = {
    css: /\.css$/,
    js: /\.js$/,
    fonts: /\.fonts$/
};

// delete the output files
function clean() {
    // async delete the automatically generated files (css folder)
    return del([paths.cssFiles, paths.jsMinFiles]);
}

// bundle sass files and output css files
function styles() {
    var retry = false;
    do {
        try {

            // get style bundles
            var sassCondition = function (file) {
                return file.history[0].indexOf('.scss') !== -1;
            };

            return getBundles(regex.css).map(function (bundle) {
                return gulp.src(bundle.inputFiles)
                    .pipe(expect(bundle.inputFiles))
                    .pipe(gulpif(sassCondition, sass()))
                    .pipe(concat('./' + bundle.outputFileName))
                    .pipe(cssmin())
                    .pipe(gulp.dest(function (f) {
                        return paths.css;
                    }));
            });
        } catch (e) {
            console.log(e);
            //await.sleep(500);
            retry = true;
        }


    } while (retry);

    return;
}

// bundle js files and output js files
function scripts() {
    var retry = false;
    do {
        try {
            // get script bundles
            return getBundles(regex.js).map(function (bundle) {
                return gulp.src(bundle.inputFiles)
                    .pipe(expect(bundle.inputFiles))
                    .pipe(concat('./' + bundle.outputFileName))
                    .pipe(gulp.dest(function (f) {
                        return paths.js;
                    }));
            });
        } catch (e) {
            console.log(e);
            //await.sleep(500);
            retry = true;
        }


    } while (retry);

    return;

}

// files to be copied to another folder
function fonts() {
    return getBundles(regex.fonts).map(function (bundle) {
        return gulp.src(bundle.inputFiles)
            .pipe(gulp.dest(paths.webfonts));
    });
}

// watch
function watch() {
    gulp.watch(paths.sassFiles, ['styles']);
    gulp.watch(paths.jsFiles, ['scripts']);
    gulp.watch(paths.fontsFiles, ['fonts']);
    console.log('Watching for files...');
}

// get all bundles with with a pattern
function getBundles(regexPattern) {
    return bundleConfig.filter(function (bundle) {
        return regexPattern.test(bundle.outputFileName);
    });
}


// tasks
gulp.task('clean', clean);
gulp.task('styles', styles);
gulp.task('scripts', scripts);
gulp.task('fonts', fonts);
gulp.task('watch', watch);
gulp.task('1-RunAndWatch', ['clean', 'styles', 'scripts', 'fonts', 'watch']);
gulp.task('2-RunOnBuild', ['clean', 'styles', 'scripts', 'fonts']);
