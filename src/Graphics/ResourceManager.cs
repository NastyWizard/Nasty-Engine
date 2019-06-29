using System.Collections.Generic;
using System.Diagnostics;
using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace NastyEngine
{
    public class ResourceManager
    {
        public static ResourceManager instance_;

        private ContentManager content;
        public static ContentManager Content { get { return instance_.content; } }

        public Dictionary<string, Texture2D> textures;
        public Dictionary<string, SoundEffect> audio;
        public Dictionary<string, Song> songs;
        public Dictionary<string, Effect> effects;


        public ResourceManager(ContentManager content)
        {
            if (instance_ == null)
            {
                instance_ = this;
                this.content = content;
                textures = new Dictionary<string, Texture2D>();
                effects = new Dictionary<string, Effect>();
                audio = new Dictionary<string, SoundEffect>();
                songs = new Dictionary<string, Song>();
            }
            else
            {
                Debug.Write("ERROR: there are 2 resource managers. fix this shit.");
            }
        }

        public static void LoadContent()
        {
            LoadTextures();
            LoadEffects();
            LoadAudio();
            LoadSongs();
        }

        public static void LoadAudio()
        {
            // TODO: Add Audio
        }

        public static void LoadSongs()
        {
            // TODO: Add Songs
        }

        public static void LoadTextures()
        {
            instance_.textures.Add("Checker", instance_.content.Load<Texture2D>("Sprites/Checker"));
        }

        public static void LoadEffects()
        {
            instance_.effects.Add("Default", instance_.content.Load<Effect>("Shaders/Default"));
            instance_.effects.Add("OutlinePP", instance_.content.Load<Effect>("Shaders/OutlinePP"));
            
            instance_.effects.Add("InvertColor", instance_.content.Load<Effect>("Shaders/InvertColor"));
            instance_.effects.Add("Circle", instance_.content.Load<Effect>("Shaders/CircleRender"));
            instance_.effects.Add("CircleOutline", instance_.content.Load<Effect>("Shaders/CircleOutlineRender"));
        }

        public static Texture2D AddTexture(string name, string location)
        {
            if (!instance_.textures.ContainsKey(name))
                instance_.textures.Add(name, instance_.content.Load<Texture2D>(location));
            return GetTexture(name);
        }

        public static Effect AddEffect(string name, string location)
        {
            if (!instance_.textures.ContainsKey(name))
                instance_.effects.Add(name, instance_.content.Load<Effect>(location));
            return GetEffect(name);
        }

        public static Song AddSong(string name, string location)
        {
            if (!instance_.textures.ContainsKey(name))
                instance_.songs.Add(name, instance_.content.Load<Song>(location));
            return GetSong(name);
        }

        public static SoundEffect AddAudio(string name, string location)
        {
            if (!instance_.textures.ContainsKey(name))
                instance_.audio.Add(name, instance_.content.Load<SoundEffect>(location));
            return GetAudio(name);
        }

        public static void AddContent<T>(string name, string location)
        {
            if (typeof(T) == typeof(Texture2D))
            {
                AddTexture(name, location);
                return;
            }
            else if (typeof(T) == typeof(Effect))
            {
                AddEffect(name, location);
                return;
            }
            else if (typeof(T) == typeof(SoundEffect))
            {
                AddAudio(name, location);
                return;
            }
            else if (typeof(T) == typeof(Song))
            {
                AddSong(name, location);
                return;
            }

            throw new System.Exception("Type or name not found 'GetContent<" + typeof(T).Name + ">(" + name + ");");
        }

        public static SoundEffect GetAudio(string name)
        {
            if (instance_.audio.ContainsKey(name))
                return instance_.audio[name];
            return null;
        }

        public static Song GetSong(string name)
        {
            if (instance_.audio.ContainsKey(name))
                return instance_.songs[name];
            return null;
        }

        public static Texture2D GetTexture(string name)
        {
            if (instance_.textures.ContainsKey(name))
                return instance_.textures[name];
            return null;
        }
        public static Effect GetEffect(string name)
        {
            if (instance_.effects.ContainsKey(name))
                return instance_.effects[name];
            return null;
        }

        public static T GetContent<T>(string name)
        {
            if (typeof(T) == typeof(Texture2D))
            {
                return (T)Convert.ChangeType(GetTexture(name), typeof(T));
            }
            else if (typeof(T) == typeof(Effect))
            {
                return (T)Convert.ChangeType(GetEffect(name), typeof(T));
            }
            else if (typeof(T) == typeof(SoundEffect))
            {
                return (T)Convert.ChangeType(GetAudio(name), typeof(T));
            }

            throw new System.Exception("Type or name not found 'GetContent<" + typeof(T).Name + ">(" + name + ");");
        }

    }
}
