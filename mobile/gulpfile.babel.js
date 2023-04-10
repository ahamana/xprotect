'use strict';

//-----------------------------------------------------------------------------
//    Modules
//-----------------------------------------------------------------------------
// Gulp 本体
import gulp from 'gulp';

// ファイルを削除するためのプラグイン
import { rimrafSync } from 'rimraf';

// Gulp 用のプラグインを一括で読み込むためのプラグイン
import gulpLoadPlugins from 'gulp-load-plugins';

//-----------------------------------------------------------------------------
//    Initialization
//-----------------------------------------------------------------------------
// 設定ファイルの読み込み
import config from './gulp-config';

// Gulp 用のプラグインの読み込み
const $ = gulpLoadPlugins();

//-----------------------------------------------------------------------------
//    Tasks
//-----------------------------------------------------------------------------
// クリーン
export const clean = () => {
    return Promise.resolve(rimrafSync(config.buildDir.dist));
};

// ビルド
export const build = () => {
    const dest = config.buildDir.dist;

    rimrafSync(`${dest}/${config.archiveName}`);

    const imageFilter = $.filter(config.filter.image.pattern, { restore: true });
    const htmlFilter = $.filter(config.filter.html.pattern, { restore: true });
    const cssFilter = $.filter('**/*.css', { restore: true });
    const tsFilter = $.filter('**/*.ts', { restore: true });
    const jsFilter = $.filter('**/*.js', { restore: true });

    return gulp.src(config.src)
        .pipe(imageFilter)
        .pipe($.libsquoosh())
        .pipe(imageFilter.restore)
        .pipe(htmlFilter)
        .pipe($.htmlmin({
            collapseWhitespace: true,
            keepClosingSlash: true,
            removeComments: true,
            ignoreCustomComments: [/^#include\s+/],
            removeStyleLinkTypeAttributes: true,
            removeScriptTypeAttributes: true,
            minifyCSS: true,
            minifyJS: true
        }))
        .pipe(htmlFilter.restore)
        .pipe(cssFilter)
        .pipe($.autoprefixer())
        .pipe($.cleanCss())
        .pipe(cssFilter.restore)
        .pipe(tsFilter)
        .pipe($.typescript.createProject(config.tsconfigJson)())
        .pipe(tsFilter.restore)
        .pipe(jsFilter)
        .pipe($.terser())
        .pipe(jsFilter.restore)
        .pipe($.zip(config.archiveName))
        .pipe(gulp.dest(dest));
};
