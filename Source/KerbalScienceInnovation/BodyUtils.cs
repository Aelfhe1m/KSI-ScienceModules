using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// based on code from https://github.com/jrossignol/ContractConfigurator/source/ContractConfigurator/ExpressionParser/Parsers/Classes/CelestialBodyParser.cs
// Contract Configurator is licensed under the MIT license 

namespace KerbalScienceInnovation
{
    public static class BodyUtils
    {
        private const double BARYCENTER_THRESHOLD = 100;

        public static CelestialBodyType BodyType(CelestialBody cb)
        {
            if (cb == null || cb.Radius < BARYCENTER_THRESHOLD)
            {
                return CelestialBodyType.NOT_APPLICABLE;
            }

            CelestialBody sun = FlightGlobals.Bodies[0];
            if (cb == sun)
            {
                return CelestialBodyType.SUN;
            }

            // Add a special case for barycenters (Sigma binary)
            if (cb.referenceBody == sun || cb.referenceBody.Radius < BARYCENTER_THRESHOLD)
            {
                // For barycenters, the biggest one is a planet, the rest are moons.
                if (cb.referenceBody.Radius < BARYCENTER_THRESHOLD)
                {
                    for (int i = cb.referenceBody.orbitingBodies.Count; --i >= 0;)
                    {
                        if (cb.referenceBody.orbitingBodies[i].Mass > cb.Mass)
                        {
                            return CelestialBodyType.MOON;
                        }
                    }
                }

                return CelestialBodyType.PLANET;
            }

            return CelestialBodyType.MOON;
        }
    }

    public enum CelestialBodyType
    {
        NOT_APPLICABLE,
        SUN,
        PLANET,
        MOON
    }
    
}
