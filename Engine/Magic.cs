﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Engine
{
    public class Magic
    {
        private string _name;
        private string _intro;
        private int _speed;
        private int _count;
        private int _region;
        private int _moveKind;
        private int _specialKind;
        private int _alphaBlend;
        private int _flyingLum;
        private int _vanishLum;
        private Asf _image;
        private Asf _icon;
        private int _waitFrame;
        private int _lifeFrame;
        private Asf _flyingImage;
        private SoundEffect _flyingSound;
        private Asf _vanishImage;
        private SoundEffect _vanishSound;
        private Asf _superModeImage;
        private int _belong;
        private string _actionFile;
        private Magic _attackFile;
        private Dictionary<int, Magic> _level;
        private int _currentLevel;
        private int _effect;
        private int _manaCost;
        private int _levelupExp;
        private bool _isOk;


        #region Public properties

        public int CurrentLevel
        {
            get { return _currentLevel; }
            set { _currentLevel = value; }
        }

        public bool IsOk
        {
            get { return _isOk; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Intro
        {
            get { return _intro; }
            set { _intro = value; }
        }

        public int Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public int Region
        {
            get { return _region; }
            set { _region = value; }
        }

        public int MoveKind
        {
            get { return _moveKind; }
            set { _moveKind = value; }
        }

        public int SpecialKind
        {
            get { return _specialKind; }
            set { _specialKind = value; }
        }

        public int AlphaBlend
        {
            get { return _alphaBlend; }
            set { _alphaBlend = value; }
        }

        public int FlyingLum
        {
            get { return _flyingLum; }
            set { _flyingLum = value; }
        }

        public int VanishLum
        {
            get { return _vanishLum; }
            set { _vanishLum = value; }
        }

        public Asf Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public Asf Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        public int WaitFrame
        {
            get { return _waitFrame; }
            set { _waitFrame = value; }
        }

        public int LifeFrame
        {
            get { return _lifeFrame; }
            set { _lifeFrame = value; }
        }

        public Asf FlyingImage
        {
            get { return _flyingImage; }
            set { _flyingImage = value; }
        }

        public SoundEffect FlyingSound
        {
            get { return _flyingSound; }
            set { _flyingSound = value; }
        }

        public Asf VanishImage
        {
            get { return _vanishImage; }
            set { _vanishImage = value; }
        }

        public Asf SuperModeImage
        {
            get { return _superModeImage; }
            set { _superModeImage = value; }
        }

        public SoundEffect VanishSound
        {
            get { return _vanishSound; }
            set { _vanishSound = value; }
        }

        public int Belong
        {
            get { return _belong; }
            set { _belong = value; }
        }

        public string ActionFile
        {
            get { return _actionFile; }
            set { _actionFile = value; }
        }

        public Magic AttackFile
        {
            get { return _attackFile; }
            set { _attackFile = value; }
        }

        public int Effect
        {
            get { return _effect; }
            set { _effect = value; }
        }

        public int ManaCost
        {
            get { return _manaCost; }
            set { _manaCost = value; }
        }

        public int LevelupExp
        {
            get { return _levelupExp; }
            set { _levelupExp = value; }
        }

        #endregion

        public Magic() { }

        public Magic(string filePath)
        {
            Load(filePath);
        }

        private void AssignToValue(string[] nameValue)
        {
            try
            {
                var info = this.GetType().GetProperty(nameValue[0]);
                switch (nameValue[0])
                {
                    case "Name":
                    case "Intro":
                    case "ActionFile":
                        info.SetValue(this, nameValue[1], null);
                        break;
                    case "Image":
                    case "Icon":
                        info.SetValue(this, Utils.GetAsf(@"asf\magic\" + nameValue[1]), null);
                        break;
                    case "FlyingImage":
                    case "VanishImage":
                    case "SuperModeImage":
                        info.SetValue(this, Utils.GetAsf(@"asf\effect\" + nameValue[1]), null);
                        break;
                    case "AttackFile":
                        if (File.Exists(@"ini\magic\" + nameValue[1]))
                            info.SetValue(this, new Magic(@"ini\magic\" + nameValue[1]), null);
                        break;
                    case "FlyingSound":
                    case "VanishSound":
                        info.SetValue(this, Utils.GetSoundEffect(nameValue[1]), null);
                        break;
                    default:
                        var integerValue = int.Parse(nameValue[1]);
                        info.SetValue(this, integerValue, null);
                        break;
                }
            }
            catch (Exception)
            {
                //Do nothing
                return;
            }
        }

        public bool Load(string filePath)
        {
            try
            {
                return Load(File.ReadAllLines(filePath, Encoding.GetEncoding(Globals.SimpleChinaeseCode)));
            }
            catch (Exception ecxeption)
            {
                Log.LogMessageToFile("Magic load failed [" + filePath + "]." + ecxeption);
                return false;
            }
        }

        public bool Load(string[] lines)
        {
            var count = lines.Count();
            var i = 0;
            var hasLevel = false;
            for (; i < count; i++)
            {
                if (Regex.Match(lines[i], @"\[Level[0-9]+\]").Groups[0].Success)
                {
                    hasLevel = true;
                    break;
                }

                var nameValue = Utils.GetNameValue(lines[i]);
                if (!string.IsNullOrEmpty(nameValue[0]))
                    AssignToValue(nameValue);
            }

            _level = null;
            if (hasLevel)
            {
                var levelList = new Dictionary<int, Magic>();
                for (var li = 1; li < 11; li++)
                {
                    var levelMagic = (Magic) this.MemberwiseClone();
                    levelMagic.CurrentLevel = li;
                    levelList.Add(li, levelMagic);
                }
                _level = levelList;

                while (i < count)
                {
                    var groups = Regex.Match(lines[i], @"\[Level([0-9]+)\]").Groups;
                    if (groups[0].Success)
                    {
                        i++;
                        int key;
                        if (int.TryParse(groups[1].Value, out key))
                        {
                            var magic = _level[key];
                            while (i < count && !string.IsNullOrEmpty(lines[i]))
                            {
                                magic.AssignToValue(Utils.GetNameValue(lines[i]));
                                i++;
                            }
                        }
                    }
                    i++;
                }
            }
            _isOk = true;
            return true;
        }

        public static void UseMagic(Character user, Magic magic, Vector2 direction)
        {
            if(user == null || magic == null) return;

            MagicManager.AddPlayerMagicSprite(new MagicSprite(
                user.PositionInWorld, 
                Globals.Basespeed * magic.Speed, 
                magic, 
                user, 
                direction));
        }
    }
}