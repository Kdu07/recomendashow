import React from "react";
import { TouchableOpacity, Image, Text, StyleSheet, View } from "react-native";
import { Heart } from "lucide-react-native";

import { colors } from "../constants/colors";
import { typography } from "../constants/typography";
import { radius, spacing } from "../constants/spacing";

export default function PosterCard({
  title,
  subtitle,
  image,
  onPress,
  isFavorite,
  onFavoritePress,
  width = 134,
  imageHeight = 186,
}) {
  return (
    <TouchableOpacity
      style={[styles.container, { width }]}
      onPress={onPress}
      activeOpacity={0.85}
    >
      <View style={[styles.imageWrapper, { height: imageHeight }]}>
        <Image source={image} style={styles.image} resizeMode="cover" />

        {onFavoritePress && (
          <TouchableOpacity
            style={styles.favoriteButton}
            activeOpacity={0.75}
            onPress={(event) => {
              event.stopPropagation();
              onFavoritePress();
            }}
          >
            <Heart
              size={16}
              strokeWidth={2.4}
              color={isFavorite ? colors.purplePrimary : colors.white}
              fill={isFavorite ? colors.purplePrimary : "transparent"}
            />
          </TouchableOpacity>
        )}
      </View>

      <Text style={styles.title} numberOfLines={1}>
        {title}
      </Text>

      {subtitle && (
        <Text style={styles.subtitle} numberOfLines={1}>
          {subtitle}
        </Text>
      )}
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  container: {
    gap: spacing.xs,
  },

  imageWrapper: {
    width: "100%",
    borderRadius: radius.md,
    overflow: "hidden",
    backgroundColor: colors.purpleSoft,
  },

  image: {
    width: "100%",
    height: "100%",
  },
  favoriteButton: {
    position: "absolute",
    top: spacing.xs,
    right: spacing.xs,
    width: 30,
    height: 30,
    borderRadius: radius.pill,
    backgroundColor: "rgba(17, 24, 39, 0.42)",
    alignItems: "center",
    justifyContent: "center",
  },

  title: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.medium,
    color: colors.grayDark,
  },

  subtitle: {
    fontSize: typography.size.xs,
    color: colors.peachBurnt,
  },
});
