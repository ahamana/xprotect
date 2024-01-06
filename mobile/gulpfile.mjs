'use strict';

//-----------------------------------------------------------------------------
//    Modules
//-----------------------------------------------------------------------------
// Gulp 本体
import gulp from 'gulp';

// ベンダープレフィックスを付与するためのプラグイン
import autoprefixer from 'gulp-autoprefixer';

// CSS を圧縮するためのプラグイン
import cleanCss from 'gulp-clean-css';

// ストリームの中身をフィルタリングするためのプラグイン
import filter from 'gulp-filter';

// HTML を圧縮するためのプラグイン
import htmlmin from 'gulp-htmlmin';

// 画像を圧縮するためのプラグイン
import imagemin from 'gulp-imagemin';

// JavaScript を圧縮するためのプラグイン
import terser from 'gulp-terser';

// TypeScript をコンパイルするためのプラグイン
import typescript from 'gulp-typescript';

// Zip 圧縮するためのプラグイン
import zip from 'gulp-zip';

// ファイルを削除するためのプラグイン
import { rimrafSync } from 'rimraf';

//-----------------------------------------------------------------------------
//    Initialization
//-----------------------------------------------------------------------------
// 設定ファイルの読み込み
import config from './gulp-config.mjs';

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

    const imageFilter = filter(config.filter.image.pattern, { restore: true });
    const htmlFilter = filter(config.filter.html.pattern, { restore: true });
    const cssFilter = filter('**/*.css', { restore: true });
    const tsFilter = filter('**/*.ts', { restore: true });
    const jsFilter = filter('**/*.js', { restore: true });

    return gulp.src(config.src)
        .pipe(imageFilter)
        .pipe(imagemin())
        .pipe(imageFilter.restore)
        .pipe(htmlFilter)
        .pipe(htmlmin({
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
        .pipe(autoprefixer())
        .pipe(cleanCss())
        .pipe(cssFilter.restore)
        .pipe(tsFilter)
        .pipe(typescript.createProject(config.tsconfigJson)())
        .pipe(tsFilter.restore)
        .pipe(jsFilter)
        .pipe(terser())
        .pipe(jsFilter.restore)
        .pipe(zip(config.archiveName))
        .pipe(gulp.dest(dest));
};
