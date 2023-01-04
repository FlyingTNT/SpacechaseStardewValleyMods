using System;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using SpaceCore.Patches;
using SpaceShared;
using StardewValley;

namespace SpaceCore
{
    public interface IApi
    {
        string[] GetCustomSkills();
        int GetLevelForCustomSkill(Farmer farmer, string skill);
        void AddExperienceForCustomSkill(Farmer farmer, string skill, int amt);
        int GetProfessionId(string skill, string profession);

        /// Must have [XmlType("Mods_SOMETHINGHERE")] attribute (required to start with "Mods_")
        void RegisterSerializerType(Type type);

        void RegisterCustomProperty( Type declaringType, string name, Type propType, MethodInfo getter, MethodInfo setter );
    }

    public class Api : IApi
    {
        public string[] GetCustomSkills()
        {
            return Skills.GetSkillList();
        }

        public int GetLevelForCustomSkill(Farmer farmer, string skill)
        {
            return Skills.GetSkillLevel(farmer, skill);
        }

        public void AddExperienceForCustomSkill(Farmer farmer, string skill, int amt)
        {
            farmer.AddCustomSkillExperience(skill, amt);
        }

        public int GetProfessionId(string skill, string profession)
        {
            return Skills.GetSkill(skill).Professions.Single(p => p.Id == profession).GetVanillaId();
        }

        public void RegisterSerializerType(Type type)
        {
            if (type.GetCustomAttribute<XmlTypeAttribute>()?.TypeName?.StartsWith("Mods_") != true)
            {
                throw new ArgumentException("Custom types must have an [XmlType] attribute with the TypeName starting with \"Mods_\"");
            }
            SpaceCore.ModTypes.Add(type);
        }

        public void RegisterCustomProperty( Type declaringType, string name, Type propType, MethodInfo getter, MethodInfo setter )
        {
            if ( !SpaceCore.CustomProperties.ContainsKey( declaringType ) )
                SpaceCore.CustomProperties.Add( declaringType, new() );

            SpaceCore.CustomProperties[ declaringType ].Add( name, new Framework.CustomPropertyInfo()
            {
                DeclaringType = declaringType,
                Name = name,
                PropertyType = propType,
                Getter = getter,
                Setter = setter,
            } );
        }
    }
}
