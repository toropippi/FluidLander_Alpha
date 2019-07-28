==========================================================================
【ソ フ ト名】	フリュードランダー　α-EDITION
【 ジャンル 】	流体アクション
【製　　　作】	toropippi,アドキチ
【動作確認１】	Windows 7,Windows 8.1,Windows 10
【動作確認２】	DirectX 11以降(シェーダーモデル 5.0以降)
			NVIDIA製：GeForce 400 Series以降
			AMD製：HD 6xxx以降
			Intel製：HD Graphics 2500/4000以降(Ivy Bridge以降)
【開発言語１】	C# (Unity)
【開発言語２】	HLSL (Compute Sheder部分)
【開発言語３】	HSP (コンフィグ部分)
===========================================================================

【インストール】
zipファイルを解凍可能なソフトで解凍して下さい。
【アンインストール】
解凍したフォルダを削除して下さい。

【ご注意！！】
こちらのゲームは一定性能以上のグラフィックボードを積んだPCでしかプレイできません！
古い世代のノートパソコンは厳しいと思います。特にintel Core iシリーズのCPUに内蔵されているGPUでは性能不足となりFPSが落ちたり、起動できない可能性があります。
所謂GPGPUを利用したゲームのためです。ご了承下さい。



【操作方法】

○キーボード
	　　　　メニュー/会話画面　　　　　アクション画面
--------------------------------------------------------	
	[Ｚ]　　　決定/会話進行　　　　　　　 燃料噴射
	[Ｘ]　　　キャンセル/会話スキップ　　 ----
　　　[←][→]　　メニュー選択 　　　　　　　 自機回転

○ゲームパッド
	　　　　メニュー/会話画面　　　　　アクション画面
--------------------------------------------------------	
　　　[button1]　　決定/会話進行　　　　　　　燃料噴射
　　　[button2]　　キャンセル/会話スキップ　　----
　　　[←][→]　　メニュー選択 　　　　　　　 自機回転

※キーボード、ゲームパッド両対応です。



【ゲーム内容】
・ゲーム背景
	このゲームは物理演算を利用した擬似的な水中で機体を操作するゲームです。
	舞台は宇宙ですので水中ではなく仮想のエーテルという空気みたいなものの中で機体を操作することになります。
・ゲームのみどころ
	見どころとしては、GPGPUを利用した高速演算による流体の挙動をお楽しみ頂ける点です。（高速かどうかはGPUに依存します）
・ゲームのすすめかた
	スタート地点からゴール地点に着地するのが目的です。レアアースを回収することでゴールが出現するステージもあります。
・スコアについて
	まず燃料は無限にあります。いくら使ってもスコアには影響しません。
	回収ミッションであるレアアースは回収すると点数が増えます。
	またクリアタイムがスコアに反映されます。早くゴールしたほうがスコアがよくなりますが、作者的には、流体の粒子の動きを楽しんで欲しいので、あまりタイムにこだり過ぎず遊んでほしいです。
	被ダメージは逆にスコアにマイナスに作用します。死亡するごとに被ダメージに大きなペナルティが加算されます。


【過去作】
本作品の過去作にあたる作品を2015年に公開しております。
「流体de月面着陸」
Vectorでダウンロードできます。


【更新履歴】
2018/11/3　α版完成
2018/8/1　 プロトタイプ完成

【クレジット】
・企画、プログラム、シナリオ　toropippi
・シナリオ、グラフィック　　　アドキチ


【謝辞】
上記以外でゲーム開発に直接協力してくださった方
・グラフィック協力	メカマスター(@koutetuoyako)
・デバッグ協力		dedp(@dedp_papiko)


以下の素材を使用させていただきました。
■背景画像提供：
１
http://www.flickr.com/photos/mattchang/3457255/ by Matt’s Life (modified by あやえも研究所)
is licensed under a Creative Commons license:
http://creativecommons.org/licenses/by/2.0/deed.en
２
玉英(ザラスドットコム)	http://material.animehack.jp/
３
きまぐれアフター		http://www5d.biglobe.ne.jp/~gakai/
４
ここから 遠い どこか	http://kokotodo.nobody.jp/
５
誰そ彼亭			http://may.force.mepage.jp/
６
ゆんフリー写真素材集	http://www.yunphoto.net/
７
明宮村（ヴィントドルフ）	http://winddorf.oops.jp/1top.htm
８
いらすとや
https://www.irasutoya.com/


■サウンド：
１
煉獄庭園			http://www.rengoku-teien.com/
２
ザ・マッチメイカァズ2nd	http://osabisi.sakura.ne.jp/m2/
３
甘茶の音楽工房		http://amachamusic.chagasi.com/


■キャラ素材：
１
暮井 慧(プロ生ちゃん)	http://pronama.azurewebsites.net/
２
同じく「プロ生ちゃん」にて
作者「池村ヒロイチ」様のWebコミックの画像を利用しております。




【免責事項】
このプログラムを使用して利用者が被った損害について、一切責任を負いかねます。
全て自己責任にてご使用をお願い致します。
（※この規約は事前告知なしに変更される場合があります）


【著作権】
C#コード、HLSLカーネルコードについてはApache Licenseである。
   Copyright 2018 toropippi

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.


【連絡先1】
efghipippi＠live.jp
【連絡先2】
twitter:Red_Black_GPGPU
http://blog.livedoor.jp/toropippi/ ブログのコメント欄まで
