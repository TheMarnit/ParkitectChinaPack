using System;
using System.Collections.Generic;
using System.IO;
using Custom_Scenery.CustomScenery;
using Custom_Scenery.CustomScenery.Decorators;
using Custom_Scenery.Decorators;
using MiniJSON;
using UnityEngine;

namespace Custom_Scenery
{
    internal class SceneryLoader : MonoBehaviour
    {
        private List<BuildableObject> _sceneryObjects = new List<BuildableObject>();

        public string Path;
		
        public Dictionary<string, object> Settings;

        public string Identifier;

        public void LoadScenery()
        {
            try
            {
                var dict = Json.Deserialize(File.ReadAllText(Path + @"/scenery.json")) as Dictionary<string, object>;
				GameObject hider = new GameObject();

                char dsc = System.IO.Path.DirectorySeparatorChar;

                using (WWW www = new WWW("file://" + Path + dsc + "assetbundle" + dsc + "scenery"))
                {
                    if (www.error != null)
                        throw new Exception("Loading had an error:" + www.error);

                    AssetBundle bundle = www.assetBundle;

                    foreach (KeyValuePair<string, object> pair in dict)
                    {
                        try
                        {
                            var options = pair.Value as Dictionary<string, object>;
							string mycategory = "";
							if((bool)Settings["chinaPackGroup"]==true) {
								mycategory += "Asia Pack";
							}
							if((bool)Settings["seperateClass"]==true) {
								if((string)options["class"]=="S") {
									mycategory += (mycategory!=""?"/":"")+"Structures";
								}
								if((string)options["class"]=="N") {
									mycategory += (mycategory!=""?"/":"")+"Nature";
								}
								if((string)options["class"]=="P") {
									mycategory += (mycategory!=""?"/":"")+"Props";
								}
							}
							if((bool)Settings["seperateGroup"]==true) {
								if((string)options["group"]=="W") {
									mycategory += (mycategory!=""?"/":"")+"Walls";
								}
								if((string)options["group"]=="F") {
									mycategory += (mycategory!=""?"/":"")+"Floors";
								}
								if((string)options["group"]=="S") {
									mycategory += (mycategory!=""?"/":"")+"Stairs";
								}
								if((string)options["group"]=="R") {
									mycategory += (mycategory!=""?"/":"")+"Roofs";
								}
								if((string)options["group"]=="P") {
									mycategory += (mycategory!=""?"/":"")+"Plants";
								}
							}
							if((bool)Settings["seperateStyle"]==true) {
								if((string)options["style"]=="TH") {
									mycategory += (mycategory!=""?"/":"")+"Teahouse";
								}
								if((string)options["style"]=="TF") {
									mycategory += (mycategory!=""?"/":"")+"Tiles";
								}
								if((string)options["style"]=="SJ") {
									mycategory += (mycategory!=""?"/":"")+"Shoji";
								}
								if((string)options["style"]=="WW") {
									mycategory += (mycategory!=""?"/":"")+"Walkway";
								}
								if((string)options["style"]=="CH") {
									mycategory += (mycategory!=""?"/":"")+"Chinese";
								}
								if((string)options["style"]=="LT") {
									mycategory += (mycategory!=""?"/":"")+"Lantern";
								}
								if((string)options["style"]=="FN") {
									mycategory += (mycategory!=""?"/":"")+"Fan";
								}
							}
							if((bool)Settings["seperateVariant"]==true) {
								if((string)options["variant"]=="R") {
									mycategory += (mycategory!=""?"/":"")+"Regular";
								} if((string)options["variant"]=="B") {
									mycategory += (mycategory!=""?"/":"")+"Bottom";
								} if((string)options["variant"]=="RC") {
									mycategory += (mycategory!=""?"/":"")+"Roof Caps";
								}
							}
							if((bool)Settings["seperateDiagonal"]==true) {
								if((string)options["diagonal"]=="R") {
									mycategory += (mycategory!=""?"/":"")+"Regular";
								} if((string)options["diagonal"]=="D") {
									mycategory += (mycategory!=""?"/":"")+"Diagonal";
								} if((string)options["diagonal"]=="S") {
									mycategory += (mycategory!=""?"/":"")+"Square";
								} if((string)options["diagonal"]=="T") {
									mycategory += (mycategory!=""?"/":"")+"Triagular";
								}
							}
                            GameObject asset = (new TypeDecorator((string)options["type"])).Decorate(options, bundle);
							asset.GetComponent<BuildableObject>().categoryTag = mycategory;
                            (new PriceDecorator((double)options["price"])).Decorate(asset, options, bundle);
                            (new NameDecorator(pair.Key)).Decorate(asset, options, bundle);

                            if (options.ContainsKey("grid"))
                                (new GridDecorator((bool)options["grid"])).Decorate(asset, options, bundle);
                            
                            if (options.ContainsKey("recolorable"))
                                (new RecolorableDecorator((bool)options["recolorable"])).Decorate(asset, options, bundle);

                            DontDestroyOnLoad(asset);

                            AssetManager.Instance.registerObject(asset.GetComponent<BuildableObject>());
                            _sceneryObjects.Add(asset.GetComponent<BuildableObject>());
                            
                            // hide it from view
                            asset.transform.parent = hider.transform;
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);

                            LogException(e);
                        }
                    }

                    bundle.Unload(false);
                }

                hider.SetActive(false);
            }
            catch(Exception e)
            {
                LogException(e);
            }
        }

        private void LogException(Exception e)
        {
            StreamWriter sw = File.AppendText(Path + @"/mod.log");

            sw.WriteLine(e);

            sw.Flush();

            sw.Close();
        }

        public void UnloadScenery()
        {
            foreach (BuildableObject deco in _sceneryObjects)
            {
                AssetManager.Instance.unregisterObject(deco);
            }
        }
    }
}
