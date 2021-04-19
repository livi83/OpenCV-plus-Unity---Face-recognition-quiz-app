/*
 Reference links_
   forum.unity.com/threads/tutorial-using-c-opencv-within-unity.459434/
   thomasmountainborn.com/2017/03/05/unity-and-opencv-part-three-passing-detection-data-to-unity/
   forum.unity.com/threads/opencvsharp-for-unity.278033/
   www.opencv-srf.com/p/introduction.html
   github.com/rajandeepsingh13/Face-Detection-using-OpenCV-CPP-in-Unity
   youtube.com/watch?v=G9QDFB2RQGA
 */

namespace OpenCvSharp.Demo
{

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using System;
    using Random = UnityEngine.Random;
    using OpenCvSharp;
    using OpenCvSharp.Face;

    public class QuizManager : MonoBehaviour
    {
        public List<QaA> QnA;

        public GameObject[] Options;
        public GameObject QuizPanel;
        public GameObject GOPanel;

        public int currentQuestion;
        public int totalQuestions = 0;
        public int score = 0;

        public Text QuestionText;
        public Text ScoreTxt;
        public RawImage Image;

        public TextAsset Faces;
        public TextAsset RecognizerXml;

        private CascadeClassifier cascadeFaces;
        private FaceRecognizer recognizer;
        string[] names;
        string faceName;

        History history = new History();

        private readonly Size requiredSize = new Size(128, 128);

        #region SK: Face recognizer trénovanie

        /*private void TrainRecognizer(string root)
		{
          /// <summary>
        /// Trénovanie recognizera obrázkami
        /// </summary>
			// OpenCV documentácia a príklady:docs.opencv.org/3.0-beta/modules/face/doc/facerec/tutorial/facerec_video_recognition.html
			// Prehľad tréningových setov:kairos.com/blog/60-facial-recognition-databases
			// ďalšia OpenCV dok: docs.opencv.org/2.4/modules/contrib/doc/facerec/facerec_tutorial.html#face-database

			int id = 0;
			var ids = new List<int>();
			var mats = new List<Mat>();
			var namesList = new List<string>();
			
			foreach (string dir in Directory.GetDirectories(root))
			{
				string name = System.IO.Path.GetFileNameWithoutExtension(dir);
				if (name.StartsWith("-"))
					continue;

				namesList.Add(name);
				UnityEngine.Debug.LogFormat("{0} = {1}", id, name);

				foreach (string file in Directory.GetFiles(dir))
				{
					var bytes = File.ReadAllBytes(file);
					var texture = new UnityEngine.Texture2D(2, 2);
					texture.LoadImage(bytes); // <--- this one has changed in Unity 2017 API and on that version must be changed

					ids.Add(id);
                    //každá načítaná textúra je konvertovaná na OpenCV Mat a daná do stupňov šedej 
					var mat = Unity.TextureToMat(texture);
					mat = mat.CvtColor(ColorConversionCodes.BGR2GRAY);
					if (requiredSize.Width > 0 && requiredSize.Height > 0)
						mat = mat.Resize(requiredSize);
					mats.Add(mat);
				}
				id++;
			}

			names = namesList.ToArray();
            //trénovanie recognizera a ukladanie výsledkov pre budúce použitie. Nie je nevyhnutné pre malé trénovacie datasety, pre veľké áno
			recognizer.Train(mats, ids);
			recognizer.Save(root + "/face-recognizer.xml");
		}*/
        #endregion
        #region ENG: Face recognizer training
        /// <summary>
        /// Routine to train face recognizer with sample images
        /// </summary>
        /*private void TrainRecognizer(string root)
		{
			// This one was actually used to train the recognizer. I didn't push much effort and satisfied once it
			// distinguished all detected faces on the sample image, for the real-world application you might want to
			// refer to the following documentation:
			// OpenCV documentation and samples: http://docs.opencv.org/3.0-beta/modules/face/doc/facerec/tutorial/facerec_video_recognition.html
			// Training sets overview: https://www.kairos.com/blog/60-facial-recognition-databases
			// Another OpenCV doc: http://docs.opencv.org/2.4/modules/contrib/doc/facerec/facerec_tutorial.html#face-database

			int id = 0;
			var ids = new List<int>();
			var mats = new List<Mat>();
			var namesList = new List<string>();
			
			foreach (string dir in Directory.GetDirectories(root))
			{
				string name = System.IO.Path.GetFileNameWithoutExtension(dir);
				if (name.StartsWith("-"))
					continue;

				namesList.Add(name);
				UnityEngine.Debug.LogFormat("{0} = {1}", id, name);

				foreach (string file in Directory.GetFiles(dir))
				{
					var bytes = File.ReadAllBytes(file);
					var texture = new UnityEngine.Texture2D(2, 2);
					texture.LoadImage(bytes); // <--- this one has changed in Unity 2017 API and on that version must be changed

					ids.Add(id);

					// each loaded texture is converted to OpenCV Mat, turned to grayscale (assuming we have RGB source) and resized
					var mat = Unity.TextureToMat(texture);
					mat = mat.CvtColor(ColorConversionCodes.BGR2GRAY);
					if (requiredSize.Width > 0 && requiredSize.Height > 0)
						mat = mat.Resize(requiredSize);
					mats.Add(mat);
				}
				id++;
			}

			names = namesList.ToArray();

			// train recognizer and save result for the future re-use, while this isn't quite necessary on small training sets, on a bigger set it should
			// give serious performance boost
			recognizer.Train(mats, ids);
			recognizer.Save(root + "/face-recognizer.xml");
		}*/
        #endregion

        /*
         SK: metóda, ktorá sa zavolá pred Update()
         ENG: method, which is called before Update()
         */
        public void Start()
        {
            totalQuestions = QnA.Count;
            GOPanel.SetActive(false);
            GenerateQuestion();
        }
        /*
        SK: Metóda, ktorá umožní návrat do menu
        ENG: Method, which allows you to go back to the menu
        */
        public void BackToMenu()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
        }
        /*
        SK: Metóda, ktorá sa zavolá pri ukončení hry
        ENG: Method, which is called when game is over
        */
        void GameOver()
        {
            QuizPanel.SetActive(false);
            GOPanel.SetActive(true);
            ScoreTxt.text = score + " / " + totalQuestions;
            CheckScore();
        
        }
        /*
        SK: Metóda, ktorá porovnáva aktuálne a uložené skóre
        ENG: Method, which compares actual and saved score
        */
        void CheckScore()
         {
             if (history.GetSavedScore() < score)
                {
                    SaveSystem.SaveScore(this);
                }
              else
                {
                    Debug.Log("Nie je co ulozit");
                }
         }
        /*
       SK: Metóda, ktorá sa zavolá, keď je odpoveď nesprávna
       ENG: Method, which is called when answer is incorrect
       */
        public void Wrong()
        {
            QnA.RemoveAt(currentQuestion);
            GenerateQuestion();
        }
        /*
       SK: Metóda, ktorá sa zavolá, keď je odpoveď správna
       ENG: Method, which is called when answer is correct
       */
        public void Correct()
        {
            score += 1;
            QnA.RemoveAt(currentQuestion);
            GenerateQuestion();
        }
        /*
       SK: Metóda, ktorá sa zavolá, pre vygenerovanie otázky
       ENG: Method, which is called for generating question
       */
        void GenerateQuestion()
        {

            if (QnA.Count > 0)
            {
                currentQuestion = Random.Range(0, QnA.Count);
                QuestionText.text = QnA[currentQuestion].Questiion;
                Image.texture = QnA[currentQuestion].sample;

                Mat image = Unity.TextureToMat(QnA[currentQuestion].sample);

                // Deteguje tváre
                var gray = image.CvtColor(ColorConversionCodes.BGR2GRAY);
                Cv2.EqualizeHist(gray, gray);
                // deteguje zhodné regióny (Faces bounding)

                FileStorage storageFaces = new FileStorage(Faces.text, FileStorage.Mode.Read | FileStorage.Mode.Memory);
                cascadeFaces = new CascadeClassifier();
                if (!cascadeFaces.Read(storageFaces.GetFirstTopLevelNode()))
                    throw new System.Exception("FaceProcessor.Initialize: Failed to load Faces cascade classifier");

                recognizer = FaceRecognizer.CreateFisherFaceRecognizer();
                recognizer.Load(new FileStorage(RecognizerXml.text, FileStorage.Mode.Read | FileStorage.Mode.Memory));
                // popisky (labels)
                names = new string[] { "Cooper", "DeGeneres", "Nyongo", "Pitt", "Roberts", "Spacey" };

                OpenCvSharp.Rect[] rawFaces = cascadeFaces.DetectMultiScale(gray, 1.1, 6);

                foreach (var faceRect in rawFaces)
                {
                    var grayFace = new Mat(gray, faceRect);
                    if (requiredSize.Width > 0 && requiredSize.Height > 0)
                        grayFace = grayFace.Resize(requiredSize);

                    int label = -1;

                    /*SK:    
                       pokús sa rozpoznať tvár:
                       confidence" je v princípe vzdialenosť od vzorky k najbližšej známej tvári
                       0 je nejaká „ideálna zhoda“
                    */
                    
                    /*ENG:    
                        now try to recognize the face:
                        confidence" here is actually a misguide. in fact, it's "distance from the sample to the closest known face".
                        0 being some "ideal match"
                    */
                    
                    double confidence = 0.0;
                    recognizer.Predict(grayFace, out label, out confidence);
                    faceName = names[label];

                    int line = 0;
                    const int textPadding = 2;
                    const double textScale = 2.0;
                    string messge = String.Format("{0}", names[label], (int)confidence);
                    var textSize = Cv2.GetTextSize(messge, HersheyFonts.HersheyPlain, textScale, 1, out line);
                    var textBox = new OpenCvSharp.Rect(
                        faceRect.X + (faceRect.Width - textSize.Width) / 2 - textPadding,
                        faceRect.Bottom,
                        textSize.Width + textPadding * 2,
                        textSize.Height + textPadding * 2
                    );
                    faceName = names[label];
                    Debug.Log(faceName);
                }
                // Priradenie obrázku textúre Image komponentu na scéne 
                // Asign image to the texture on Image component
                var texture = Unity.MatToTexture(image);
                var rawImage = Image;
                rawImage.texture = texture;

                var transform = Image.GetComponent<RectTransform>();
                transform.sizeDelta = new Vector2(image.Width, image.Height);

                for (int i = 0; i < Options.Length; i++)
                {
                    Options[i].transform.GetChild(0).GetComponent<Text>().text = QnA[currentQuestion].Answers[i];
                    if (faceName == Options[i].transform.GetChild(0).GetComponent<Text>().text)
                    {
                        Options[i].GetComponent<AnswerScript>().isCorrect = true;
                    }
                    else
                    {
                        Options[i].GetComponent<AnswerScript>().isCorrect = false;
                    }
                }
            }
            else
            {
                GameOver();
            }
        }
    }
}
