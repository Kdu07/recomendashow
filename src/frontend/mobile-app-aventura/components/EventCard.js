import React from 'react';
import { View, Text, Image, StyleSheet, TouchableOpacity } from 'react-native';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { radius, spacing } from '../constants/spacing';

export default function EventCard({
  title,
  location,
  category,
  date,
  image,
  onPress,
  onFavoritePress,
  isFavorite = false,
}) {
  return (
    <TouchableOpacity
      style={styles.card}
      onPress={onPress}
      activeOpacity={0.85}
    >
      <Image
        source={image}
        style={styles.image}
        resizeMode="cover"
      />

      <TouchableOpacity
        style={styles.favoriteButton}
        onPress={onFavoritePress}
        activeOpacity={0.8}
      >
        <Text style={styles.favoriteIcon}>
          {isFavorite ? '♥' : '♡'}
        </Text>
      </TouchableOpacity>

      <View style={styles.content}>
        {category && (
          <Text style={styles.category}>
            {category}
          </Text>
        )}

        <Text style={styles.title} numberOfLines={2}>
          {title}
        </Text>

        {location && (
          <Text style={styles.info} numberOfLines={1}>
            {location}
          </Text>
        )}

        {date && (
          <Text style={styles.date} numberOfLines={1}>
            {date}
          </Text>
        )}
      </View>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  card: {
    width: 200,
    borderRadius: radius.lg,
    backgroundColor: colors.white,
    overflow: 'hidden',
    borderWidth: 1,
    borderColor: colors.grayLight,
  },

  image: {
    width: '100%',
    height: 150,
    backgroundColor: colors.purpleSoft,
  },

  favoriteButton: {
    position: 'absolute',
    top: spacing.sm,
    right: spacing.sm,
    width: 32,
    height: 32,
    borderRadius: radius.pill,
    backgroundColor: colors.white,
    alignItems: 'center',
    justifyContent: 'center',
  },

  favoriteIcon: {
    fontSize: typography.size.lg,
    color: colors.purplePrimary,
  },

  content: {
    padding: spacing.md,
    gap: spacing.xs,
  },

  category: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.purplePrimary,
  },

  title: {
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  info: {
    fontSize: typography.size.sm,
    color: colors.grayText,
  },

  date: {
    fontSize: typography.size.xs,
    color: colors.grayText,
  },
});