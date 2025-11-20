using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//------------------------------------------------------
//
//  RayCast処理
//
//------------------------------------------------------
public class CRayCastProc : MonoBehaviour
{   
    //--------------------------------------
    //  RayCastAllRtn
    //  当たったオブジェクトを全て返す
    //--------------------------------------

    public static GameObject[] RaycastAllRtn(Vector2 wMusePos)
    {
        // マウス位置をRayに変換
        Ray wRayAct = Camera.main.ScreenPointToRay(wMusePos);
        //　マウス位置からRayを飛ばして当たったオブジェクトを全て取得
        RaycastHit2D[] wHitOBJ = Physics2D.RaycastAll(wRayAct.origin, wRayAct.direction);
        //　wHitOBJの長さを取得
        int wHitLen = wHitOBJ.Length;
        //　当たったオブジェクトを格納する配列を生成
        GameObject[] wOBJ = new GameObject[wHitLen];
        if(wHitLen >= 1)
        {
            //  当たったオブジェクトを配列に格納
            int wOBJIx = 0;
            for(int i = 0; i < wHitLen; i++)
            {
                wOBJ[wOBJIx] = wHitOBJ[i].collider.gameObject;
            }
        }
        return wOBJ;
    }
    //--------------------------------------
    //  RayCastSortingOrdor
    //  マウスの当たったオブジェクトの中で最も手前にあるオブジェクトを返す
    //--------------------------------------

    public static GameObject RayCastSortingOrdor(Vector2 wMusePos)
    {
        // マウスの位置をRayに変換
        Ray wRayAct = Camera.main.ScreenPointToRay(wMusePos);
        // マウス位置からRayを飛ばして当たったオブジェクトを全て取得
        RaycastHit2D[] wHitOBJ = Physics2D.RaycastAll(wRayAct.origin, wRayAct.direction);
        // Hitしたオブジェクトの数を取得
        int wHitlen = wHitOBJ.Length;
        //最奥のsortingOrderを格納する変数
        int wSortMax = -999;
        GameObject wRet = null;

        if(wHitlen >= 1)
        {
            for(int i = 0 ; i < wHitlen; i++){
                //i番目のオブジェクトを取得
                GameObject wObj = wHitOBJ[i].collider.gameObject;
                string wName = wObj.name;
                // GetCompornent<Tipe>()でSpriteRendererを取得し、sortingOrderでｗOBJの描画順を取得（-100が奥、100が手前）
                int wSort = wObj.GetComponent<SpriteRenderer>().sortingOrder;
                if(wSort > wSortMax){
                    wSortMax = wSort;
                    wRet = wObj;
                }
            }
        }
        return wRet;
    }
    //--------------------------------------
    //　RayCastFrontZ
    //  マウスの当たったオブジェクトの中で最も手前にあるオブジェクトを返す(Z座標)
    public static GameObject RayCastFrontZ(Vector2 wMusePos)
    {
        // マウスの位置をRayに変換
        Ray wRayAct = Camera.main.ScreenPointToRay(wMusePos);
        // マウス位置からRayを飛ばして当たったオブジェクトを全て取得
        RaycastHit2D[] wHitOBJ = Physics2D.RaycastAll(wRayAct.origin, wRayAct.direction);
        // Hitしたオブジェクトの数を取得
        int wHitlen = wHitOBJ.Length;
        //最奥のZ座標を格納する変数
        float wZMax = -999f;
        GameObject wRet = null;

        if(wHitlen >= 1)
        {
            for(int i = 0 ; i < wHitlen; i++){
                //i番目のオブジェクトを取得
                GameObject wObj = wHitOBJ[i].collider.gameObject;
                string wName = wObj.name;
                // Z座標を取得
                float wZ = wObj.transform.position.z;
                if(wZ > wZMax){
                    wZMax = wZ;
                    wRet = wObj;
                }
            }
        }
        return wRet;
    }

}
