'use strict';

//-----------------------------------------------------------------------------
//    Constants
//-----------------------------------------------------------------------------
// ビルド用のルートディレクトリ
const BUILD_ROOT_DIR = './build';

// ビルド用のディレクトリ
const BUILD_DIR = {
	root: BUILD_ROOT_DIR,
	dist: `${BUILD_ROOT_DIR}/distributions`
}

// コンテンツのディレクトリ
const CONTENTS_DIR = './Samples';

// コンテンツファイルのパス
const CONTENT_FILE_PATHS = `${CONTENTS_DIR}/**/*`;

// 画像ファイルのパターン
const IMAGE_FILE_PATTERN = ['**/*.jpg', '**/*.png'];

// HTML ファイルのパターン
const HTML_FILE_PATTERN = ['**/*.html', '**/*.ssi'];

// tsconfig.json ファイルのパス
const TSCONFIG_JSON_FILE_PATH = './tsconfig.json';

//-----------------------------------------------------------------------------
//    Build Settings
//-----------------------------------------------------------------------------
export default {
	// tsconfig.json
	tsconfigJson: TSCONFIG_JSON_FILE_PATH,

	// ビルドディレクトリ
	buildDir: BUILD_DIR,

	// コンテンツファイル
	src: CONTENT_FILE_PATHS,

	// フィルタ
	filter: {
		// 画像
		image: {
			pattern: IMAGE_FILE_PATTERN
		},

		// HTML
		html: {
			pattern: HTML_FILE_PATTERN
		}
	},

	// アーカイブ名
	archiveName: 'Samples.zip'
}
