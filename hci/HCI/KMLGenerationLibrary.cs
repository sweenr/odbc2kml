﻿using System;


namespace HCI
{
    public class KMLGenerationLibrary
    {
        //XML formatted KML
        private String formattedKML;

        //Constructor

        /// <summary>
        /// Initialize basic KML and accept the desired KML file 
        /// name
        /// </summary>
        /// <param name="kmlFileName">Desired file name</param>
        public KMLGenerationLibrary(String kmlFileName)
        {
            //Initialize KML basics
            initializeKML(kmlFileName);
        }

        //Functions

        /// <summary>
        /// Adds the basic necessities needed for a KML file to the KML string
        /// </summary>
        /// <param name="kmlFileName">Desired file name</param>
        public void initializeKML(String kmlFileName)
        {
            //XML/KML initialization
            formattedKML =
            "<?xml version='1.0' encoding='UTF-8'?>\n" +
            "<kml xmlns='http://www.opengis.net/kml/2.2' xmlns:gx='http://www.google.com/kml/ext/2.2' xmlns:kml='http://www.opengis.net/kml/2.2' xmlns:atom='http://www.w3.org/2005/Atom'>\n" +
            "<Document>\n" +
            "\t<name>" + kmlFileName + "</name>\n";

        }

        /// <summary>
        /// Returns the final string containing the KML file
        /// </summary>
        /// <returns>formattedKML --> class String</returns>
        public String finalizeKML()
        {
            formattedKML += 
                "</Document>\n" +
                "</kml>";

            return formattedKML;
        }

        /// <summary>
        /// Adds a placemark to the KML file and associates the placemark with
        /// latitude and longitude coordinates, a description, a name, and a desired style
        /// </summary>
        /// <param name="name">String --> placemark name</param>
        /// <param name="description">String --> placemark description</param>
        /// <param name="lat">double --> Latitude</param>
        /// <param name="lon">double --> Longitude</param>
        /// <param name="styleName">String --> Style Name</param>
        public void addPlacemark(Placemark place)
        {
            formattedKML +=
                "\t<Placemark>\n" +
                "\t\t<name>" + place.getPlacemarkName() + "</name>\n" +
                "\t\t<description>\n" +
                "\t\t\t<![CDATA[" + place.getPlacemarkDescription() + "]]>\n" +
                "\t\t</description>\n";
            if (place.getPlacemarkStyleName().Length != 0)
            {
                formattedKML += "\t\t\t<styleUrl>" + place.getPlacemarkStyleName() + "</styleUrl>\n";
            }
            formattedKML +=       
                "\t\t\t<Point>\n" +
                "\t\t\t\t<altitudeMode>clampToGround</altitudeMode>\n" +
                "\t\t\t\t<coordinates>" + place.getPlacemarkLongitude() + "," + place.getPlacemarkLatitude() + "</coordinates>\n" +
                "\t\t\t</Point>\n" +
                "\t</Placemark>\n";
        }


        /// <summary>
        /// Adds a style to the KML file. Requires the icon ID and color. 
        /// </summary>
        /// <param name="ic">Icon --> needed to retrieve icon ID and icon location</param>
        /// <param name="color">String --> color of the overlay </param>
        /// <param name="styleName">String --> associated styleName</param>
        public void addStyle(Style style)
        {
            if (style.getStyleColor() == 0) //Don't add a color
            {
                if (style.getStyleIcon().getLocation() == "")
                {
                    formattedKML +=
                       "";
                }
                else
                {
                    formattedKML +=
                        "\t<Style id='" + style.getStyleName() + "'>\n" +
                        "\t<IconStyle>\n" +
                        "\t\t<Icon>\n" +
                        "\t\t\t<href>" + style.getStyleIcon().getLocation() + "</href>\n" +
                        "\t\t</Icon>\n" +
                        "\t</IconStyle>\n" +
                        "\t</Style>\n";
                }
            }
            else //Add a color
            {
                if (style.getStyleIcon().getLocation() == "")
                {
                    formattedKML +=
                        "\t<Style id='" + style.getStyleName() + "'>\n" +
                        "\t<IconStyle>\n" +
                        "\t\t<color>" + style.getStyleColor().ToString("X") + "</color>\n" +
                        "\t</IconStyle>\n" +
                        "\t</Style>\n";
                }
                else
                {
                    formattedKML +=
                        "\t<Style id='" + style.getStyleName() + "'>\n" +
                        "\t<IconStyle>\n" +
                        "\t\t<color>" + style.getStyleColor().ToString("X") + "</color>\n" +
                        "\t\t<Icon>\n" +
                        "\t\t\t<href>" + style.getStyleIcon().getLocation() + "</href>\n" +
                        "\t\t</Icon>\n" +
                        "\t</IconStyle>\n" +
                        "\t</Style>\n";
                }
            }
        }
    }
}
