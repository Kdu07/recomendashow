import React from 'react';
import { View, Text, Image, ScrollView, StyleSheet } from 'react-native';

import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { spacing, radius } from '../../../constants/spacing';

function RecommendationCard({ item }) {
  return (
    <View style={styles.card}>
      <Image source={item.image} style={styles.cardImage} resizeMode="cover" />
      <Text style={styles.cardTitle} numberOfLines={2}>{item.title}</Text>
    </View>
  );
}

export default function ShowRecommendations({ recommendations }) {
  return (
    <View style={styles.container}>
      <View style={styles.divider} />

      <View style={styles.header}>
        <Text style={styles.title}>Você também pode gostar</Text>
        <Text style={styles.subtitle}>Espetáculos semelhantes a esse</Text>
      </View>

      <ScrollView
        horizontal
        showsHorizontalScrollIndicator={false}
        contentContainerStyle={styles.list}
      >
        {recommendations.map((item) => (
          <RecommendationCard key={item.id} item={item} />
        ))}
      </ScrollView>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    paddingTop: spacing.xl,
    gap: spacing.md,
  },

  divider: {
    height: 1,
    backgroundColor: colors.grayLight,
    marginHorizontal: spacing.xl,
    marginBottom: spacing.sm,
  },

  header: {
    paddingHorizontal: spacing.xl,
    gap: 2,
  },

  title: {
    fontSize: 17,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  subtitle: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
  },

  list: {
    paddingHorizontal: spacing.xl,
    gap: spacing.md,
    paddingBottom: spacing.sm,
  },

  card: {
    width: 120,
    gap: spacing.xs,
  },

  cardImage: {
    width: 120,
    height: 160,
    borderRadius: radius.md,
  },

  cardTitle: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.medium,
    color: colors.grayDark,
  },
});
